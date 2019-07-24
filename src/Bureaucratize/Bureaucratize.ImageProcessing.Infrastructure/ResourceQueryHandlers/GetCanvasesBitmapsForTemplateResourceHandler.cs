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
    public class GetCanvasesBitmapsForTemplateResourceHandler 
        : IResourceQueryHandler<GetCanvasesBitmapsForTemplate, FileStorageRequestResult<ICollection<TemplatePageCanvasBitmapResource>>>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;

        public GetCanvasesBitmapsForTemplateResourceHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null)
                throw new ArgumentNullException(nameof(persistenceConfiguration));
            _persistenceConfiguration = persistenceConfiguration;
        }

        public FileStorageRequestResult<ICollection<TemplatePageCanvasBitmapResource>> Handle(GetCanvasesBitmapsForTemplate command)
        {
            var url = _persistenceConfiguration.FileStorageApiTemplateFilesUrl
                      + $"/template-definition/{command.TemplateId}/canvases";

            return url.GetJsonAsync<FileStorageRequestResult<ICollection<TemplatePageCanvasBitmapResource>>>().Result;
        }
    }
}
