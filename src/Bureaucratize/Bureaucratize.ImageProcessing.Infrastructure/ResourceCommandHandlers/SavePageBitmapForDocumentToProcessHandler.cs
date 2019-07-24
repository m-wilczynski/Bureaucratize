using System;
using Bureaucratize.Common.Core.Infrastructure;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Commands;
using Flurl.Http;

namespace Bureaucratize.ImageProcessing.Infrastructure.ResourceCommandHandlers
{
    public class SavePageBitmapForDocumentToProcessHandler
        : IResourceCommandHandler<SavePageBitmapForDocumentToProcess, FileStorageRequestResult>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;

        public SavePageBitmapForDocumentToProcessHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null)
                throw new ArgumentNullException(nameof(persistenceConfiguration));

            _persistenceConfiguration = persistenceConfiguration;
        }

        public FileStorageRequestResult Handle(SavePageBitmapForDocumentToProcess command)
        {
            var result = $"{_persistenceConfiguration.FileStorageApiUserDocumentsUrl}/document-to-process-page/"
                .PostJsonAsync(command).ReceiveJson<FileStorageRequestResult>().Result;

            return result;
        }
    }
}
