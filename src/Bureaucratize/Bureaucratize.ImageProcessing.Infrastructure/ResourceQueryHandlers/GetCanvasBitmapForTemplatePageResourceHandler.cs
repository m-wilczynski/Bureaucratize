using System;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Models;
using Bureaucratize.FileStorage.Contracts.Queries;
using Flurl.Http;

namespace Bureaucratize.ImageProcessing.Infrastructure.ResourceQueryHandlers
{
    public class GetCanvasBitmapForTemplatePageResourceHandler 
        : IResourceQueryHandler<GetCanvasBitmapForTemplatePage, TemplatePageCanvasBitmapResource>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;

        public GetCanvasBitmapForTemplatePageResourceHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null)
                throw new ArgumentNullException(nameof(persistenceConfiguration));
            _persistenceConfiguration = persistenceConfiguration;
        }

        public TemplatePageCanvasBitmapResource Handle(GetCanvasBitmapForTemplatePage command)
        {
            var url = _persistenceConfiguration.FileStorageApiTemplateFilesUrl
                      + $"/template-page/{command.TemplatePageId}/canvas";

            return url.GetJsonAsync<FileStorageRequestResult<TemplatePageCanvasBitmapResource>>().Result.Result;
        }
    }
}
