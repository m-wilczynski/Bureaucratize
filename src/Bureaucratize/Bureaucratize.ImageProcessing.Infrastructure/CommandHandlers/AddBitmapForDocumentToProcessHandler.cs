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
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Common.Core.Infrastructure;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.Common.Core.Infrastructure.ResultMessages;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Commands;
using Bureaucratize.ImageProcessing.Contracts.Bitmaps;
using Bureaucratize.ImageProcessing.Core.Commands;
using Bureaucratize.ImageProcessing.Core.Common;
using Bureaucratize.ImageProcessing.Core.Document;
using Bureaucratize.ImageProcessing.Core.Queries;

namespace Bureaucratize.ImageProcessing.Infrastructure.CommandHandlers
{
    public class AddBitmapForDocumentToProcessHandler : ICommandHandler<AddBitmapForDocumentToProcess, bool>
    {
        private readonly IResourceCommandHandler<SavePageBitmapForDocumentToProcess, FileStorageRequestResult> _saveImageCommand;
        private readonly IQueryHandler<GetDocumentToProcess, DocumentToProcess> _getDocumentToProcess;

        public AddBitmapForDocumentToProcessHandler(
            IQueryHandler<GetDocumentToProcess, DocumentToProcess> getDocumentToProcess,
            IResourceCommandHandler<SavePageBitmapForDocumentToProcess, FileStorageRequestResult> persistImageCommand)
        {
            if (getDocumentToProcess == null) throw new ArgumentNullException(nameof(getDocumentToProcess));
            if (persistImageCommand == null) throw new ArgumentNullException(nameof(persistImageCommand));

            _saveImageCommand = persistImageCommand;
            _getDocumentToProcess = getDocumentToProcess;
        }

        public OperationResult<bool> Handle(AddBitmapForDocumentToProcess command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = _getDocumentToProcess.Handle(new GetDocumentToProcess {DocumentId = command.DocumentId});

            if (!result.Successful)
            {
                //TODO: QueryFailed : IResultDetails
                return OperationResult<bool>.Failure(null);
            }

            var document = result.Result;
            var documentModificationResult = document.AddDocumentPageBitmap(command.OrderedBitmap.AsOrderedBitmap());

            if (!documentModificationResult.Successful)
            {
                //TODO: DocumentModificationFailed (+ reason)
                return OperationResult<bool>.Failure(null);
            }

            return SaveImageToFileStorage(command);
        }

        private OperationResult<bool> SaveImageToFileStorage(AddBitmapForDocumentToProcess command)
        {
            try
            {
                var fileStoreCommand = new SavePageBitmapForDocumentToProcess
                {
                    DocumentId = command.DocumentId,
                    PageNumber = command.OrderedBitmap.Order,
                    FileData = command.OrderedBitmap.FileData,
                    FileLabel = command.OrderedBitmap.FileLabel,
                    FileType = command.OrderedBitmap.FileType
                };

                var persistResult = _saveImageCommand.Handle(fileStoreCommand);

                if (!persistResult.Success)
                {
                    return OperationResult<bool>.Failure(new FileStorageSaveFailed(fileStoreCommand));
                }
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure(new UncaughtException(ex));
            }

            return OperationResult<bool>.Success(true);
        }
    }
}
