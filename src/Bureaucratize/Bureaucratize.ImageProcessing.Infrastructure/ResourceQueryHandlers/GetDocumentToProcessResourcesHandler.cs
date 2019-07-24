using System;
using System.Collections.Generic;
using Bureaucratize.Common.Core.Infrastructure;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Models;
using Bureaucratize.FileStorage.Contracts.Queries;
using Flurl.Http;

namespace Bureaucratize.ImageProcessing.Infrastructure.ResourceQueryHandlers
{
    public class GetDocumentToProcessResourcesHandler : 
        IResourceQueryHandler<GetBitmapsForDocumentToProcess, FileStorageRequestResult<ICollection<OrderedBitmapResource>>>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;

        public GetDocumentToProcessResourcesHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null) throw new ArgumentNullException(nameof(persistenceConfiguration));
            _persistenceConfiguration = persistenceConfiguration;
        }

        public FileStorageRequestResult<ICollection<OrderedBitmapResource>> Handle(GetBitmapsForDocumentToProcess command)
        {
            var url = _persistenceConfiguration.FileStorageApiUserDocumentsUrl 
                      + $"/document-to-process/{command.DocumentId}";

            return url.GetJsonAsync<FileStorageRequestResult<ICollection<OrderedBitmapResource>>>().Result;
        }
    }
}
