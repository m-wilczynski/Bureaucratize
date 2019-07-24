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

using Accord.Imaging;
using Accord.Imaging.Filters;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.Common.Core.Utils;
using Bureaucratize.FileStorage.Contracts.Models;
using Bureaucratize.FileStorage.Contracts.Queries;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.Details;
using Bureaucratize.ImageProcessing.Core.Extracting.Contracts;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Core.Template.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Extracting
{
    public class ImageDifferenceHandwrittenInputExtractor : IHandwrittenInputExtractor
    {
        private readonly IResourceQueryHandler<GetCanvasBitmapForTemplatePage, TemplatePageCanvasBitmapResource> _canvasBitmapQueryHandler;

        public ImageDifferenceHandwrittenInputExtractor(
            IResourceQueryHandler<GetCanvasBitmapForTemplatePage, TemplatePageCanvasBitmapResource> canvasBitmapQueryHandler)
        {
            if (canvasBitmapQueryHandler == null)
                throw new ArgumentNullException(nameof(canvasBitmapQueryHandler));

            _canvasBitmapQueryHandler = canvasBitmapQueryHandler;
        }

        public ProcessingResult<Bitmap> ExtractHandwrittenInput(Bitmap userInput, ITemplatePageDefinition templatePage)
        {
            if (userInput == null || templatePage == null)
            {
                return ProcessingResult<Bitmap>.Failure(new EmptyInput());
            }

            using (userInput)
            {
                Bitmap canvasBitmap = null;

                try
                {
                    canvasBitmap = _canvasBitmapQueryHandler.Handle(
                        new GetCanvasBitmapForTemplatePage { TemplatePageId = templatePage.Id }).FileData.AsBitmap();

                    if (canvasBitmap == null)
                    {
                        //TODO: ResultMessage for failed HTTP request or pipe from ResourceQueryHandler
                        return new ProcessingResult<Bitmap>(StepOutcome.Failure, null, null);
                    }

                    if (canvasBitmap.Size != userInput.Size)
                    {
                        using (var oldCanvas = canvasBitmap)
                        {
                            canvasBitmap = new ResizeBilinear(userInput.Width, userInput.Height).Apply(oldCanvas);
                        }
                    }

                    var result = new Invert().Apply(new Difference(userInput).Apply(canvasBitmap));

                    return ProcessingResult<Bitmap>.Success(result);

                }
                catch (UnsupportedImageFormatException)
                {
                    return ProcessingResult<Bitmap>.Failure(new UnsupportedImageFormat());
                }
                finally
                {
                    canvasBitmap?.Dispose();
                }
            }
        }
    }
}
