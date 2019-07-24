/*
   Copyright (c) 2018 Michał Wilczyński

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Common.Core.Infrastructure.Common.Shortcuts;
using Bureaucratize.ImageProcessing.Contracts.Bitmaps;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes.Models;
using Bureaucratize.ImageProcessing.Contracts.Recognition;
using Bureaucratize.ImageProcessing.Contracts.RemotingMessages;
using Bureaucratize.ImageProcessing.Core.Cleaning;
using Bureaucratize.ImageProcessing.Core.Cropping.Contracts;
using Bureaucratize.ImageProcessing.Core.Document;
using Bureaucratize.ImageProcessing.Core.Queries;
using Bureaucratize.ImageProcessing.Core.Recognition.Contracts;
using Bureaucratize.ImageProcessing.Infrastructure;
using Bureaucratize.Templating.Core.Template;

namespace Bureaucratize.ImageProcessing.Host.Actors.ImageProcessing
{
    public class DocumentImagesProcessor : ReceiveActor
    {
        protected IImageProcessingPersistenceConfiguration PersistenceConfiguration { get; }
        private readonly IQueryHandler<GetDocumentToProcess, DocumentToProcess> _getDocumentHandler;
        private readonly ICommandHandler<SaveDomainObject<ProcessedDocumentPage>> _saveProcessedPageHandler;
        private readonly ImageProcessingPreparationSteps _imagePreparationSteps;
        private readonly IHandwrittenInputRecognizer _handwrittenInputRecognizer;
        private readonly IHandwrittenChoiceRecognizer _handwrittenChoiceRecognizer;
        private Task _runningTask;
        private CancellationTokenSource _tokenSource;

        public DocumentImagesProcessor(IQueryHandler<GetDocumentToProcess, DocumentToProcess> getDocumentHandler,
            IImageProcessingPersistenceConfiguration persistenceConfiguration,
            ICommandHandler<SaveDomainObject<ProcessedDocumentPage>> saveProcessedPageHandler,
            ImageProcessingPreparationSteps imagePreparationSteps,
            IHandwrittenInputRecognizer handwrittenInputRecognizer,
            IHandwrittenChoiceRecognizer handwrittenChoiceRecognizer)
        {
            ValidateConstructionParams(getDocumentHandler, persistenceConfiguration, saveProcessedPageHandler, 
                imagePreparationSteps, handwrittenInputRecognizer, handwrittenChoiceRecognizer);

            PersistenceConfiguration = persistenceConfiguration;
            _getDocumentHandler = getDocumentHandler;
            _saveProcessedPageHandler = saveProcessedPageHandler;
            _imagePreparationSteps = imagePreparationSteps;
            _handwrittenInputRecognizer = handwrittenInputRecognizer;
            _handwrittenChoiceRecognizer = handwrittenChoiceRecognizer;

            _tokenSource = new CancellationTokenSource();

            Ready();
        }

        private void Ready()
        {
            Receive<ProcessDocumentOfIdRequest>(req =>
            {

                var self = Self;
                var sender = Sender;
                var request = req;

                _runningTask = Task.Run(() =>
                    {
                        var getDocumentResult = _getDocumentHandler.Handle(new GetDocumentToProcess
                        {
                            DocumentId = request.DocumentId
                        });

                        if (!getDocumentResult.Successful)
                        {
                            sender.Tell(new DocumentProcessingFailed(request.DocumentId));
                            return;
                        }

                        foreach (var page in getDocumentResult.Result.DocumentPages)
                        {
                            ProcessPage(page, sender, getDocumentResult.Result);
                        }

                    }, _tokenSource.Token)
                    .ContinueWith<DocumentMessage>(x =>
                    {
                        if (x.IsCanceled || x.IsFaulted)
                            return new DocumentProcessingFailed(request.DocumentId);

                        return new DocumentProcessingCompleted(request.DocumentId);
                    }, TaskContinuationOptions.ExecuteSynchronously)
                    .PipeTo(sender);

            });
        }

        private void ProcessPage(OrderedBitmap page, IActorRef senderRef, DocumentToProcess document)
        {
            senderRef.Tell(new DocumentPageProcessingStarted(document.Id, (int)page.Order));
            var pageDefinition = document.TemplateDefinition.DefinedPages[(int)page.Order];

            //Extracting from input only parts that are handwritten elements (ie. delta between empty document and filled by hand document) 
            var extractedParts = _imagePreparationSteps.HandwrittenInputExtractor.ExtractHandwrittenInput(page.Bitmap, pageDefinition);
            //And cropping them according to defined template
            var croppingResult = _imagePreparationSteps.TemplateAreasCropper.CropUserInput(extractedParts.Result, document.Id, pageDefinition);

            //Processed parts
            var processedTextAreas = new List<RecognizedTextPart>();
            var processedChoiceAreas = new List<RecognizedChoicePart>();

            foreach (var croppedPart in croppingResult.Result)
            {
                senderRef.Tell(new DocumentPageAreaProcessingStarted(document.Id, (int)page.Order, croppedPart.AreaUsedForCropping.AreaName));

                var cleanResult = _imagePreparationSteps.CroppedAreaCleaner.CleanUp(croppedPart).Result;
                var roiExtractResult = _imagePreparationSteps.RegionOfInterestExtractor.ExtractRegionOfInterestFrom(cleanResult).Result;

                //Choice part, ie. checkbox on paper
                if (croppedPart.AreaUsedForCropping.ExpectedData == TemplatePartExpectedDataType.Choice)
                {
                    processedChoiceAreas.Add(RecognizeChoicePart(page, senderRef, document, croppedPart, roiExtractResult));
                }
                //Text part, ie. field to fill with name/surname/personal id etc.
                else
                {
                    var downscaleResult = _imagePreparationSteps.CroppedAreaScaler.ScaleAndFlatten(roiExtractResult).Result;
                    processedTextAreas.Add(RecognizeTextPart(page, senderRef, document, croppedPart, downscaleResult));
                }
            }

            var processedPage = new ProcessedDocumentPage(processedTextAreas, processedChoiceAreas, (int)page.Order);

            var savePageResult = _saveProcessedPageHandler.Handle(processedPage.AsSaveCommand());

            if (savePageResult.Successful)
            {
                senderRef.Tell(new DocumentPageProcessingCompleted(document.Id, processedPage));
            }
            //TODO: Create failure communication, ie. selfRef.Tell(new Failure())
            else
            {
                Console.WriteLine("Failed to save recognized parts for page: " + page.Order + "of document: " + document.Id);
            }
        }

        private RecognizedChoicePart RecognizeChoicePart(OrderedBitmap page, IActorRef senderRef, DocumentToProcess document, 
            ICroppedArea croppedPart, ICroppedArea roiExtractResult)
        {
            var recognitionResult = _handwrittenChoiceRecognizer.RecognizeFrom(roiExtractResult).Result as RecognizedChoicePart;
            recognitionResult.AreaName = croppedPart.AreaUsedForCropping.AreaName;

            senderRef.Tell(new DocumentPageChoiceAreaProcessingCompleted(document.Id, (int)page.Order,
                croppedPart.AreaUsedForCropping.AreaName, recognitionResult));

            return recognitionResult;
        }

        private RecognizedTextPart RecognizeTextPart(OrderedBitmap page, IActorRef selfRef, DocumentToProcess document, 
            ICroppedArea croppedPart, FlattenedCroppedArea downscaleResult)
        {
            var recognitionResult = _handwrittenInputRecognizer.RecognizeFrom(downscaleResult).Result;
            recognitionResult.AreaName = croppedPart.AreaUsedForCropping.AreaName;
            
            selfRef.Tell(new DocumentPageTextAreaProcessingCompleted(document.Id, (int)page.Order,
                    croppedPart.AreaUsedForCropping.AreaName, recognitionResult));

            return recognitionResult as RecognizedTextPart;
        }

        private static void ValidateConstructionParams(
            IQueryHandler<GetDocumentToProcess, DocumentToProcess> getDocumentHandler,
            IImageProcessingPersistenceConfiguration persistenceConfiguration,
            ICommandHandler<SaveDomainObject<ProcessedDocumentPage>> saveProcessedPageHandler,
            ImageProcessingPreparationSteps imagePreparationSteps,
            IHandwrittenInputRecognizer handwrittenInputRecognizer,
            IHandwrittenChoiceRecognizer handwrittenChoiceRecognizer)
        {
            if (getDocumentHandler == null)
                throw new ArgumentNullException(nameof(getDocumentHandler));
            if (persistenceConfiguration == null)
                throw new ArgumentNullException(nameof(persistenceConfiguration));
            if (saveProcessedPageHandler == null)
                throw new ArgumentNullException(nameof(saveProcessedPageHandler));
            if (imagePreparationSteps == null)
                throw new ArgumentNullException(nameof(imagePreparationSteps));
            if (handwrittenInputRecognizer == null)
                throw new ArgumentNullException(nameof(handwrittenInputRecognizer));
            if (handwrittenChoiceRecognizer == null)
                throw new ArgumentNullException(nameof(handwrittenChoiceRecognizer));
        }
    }
}
