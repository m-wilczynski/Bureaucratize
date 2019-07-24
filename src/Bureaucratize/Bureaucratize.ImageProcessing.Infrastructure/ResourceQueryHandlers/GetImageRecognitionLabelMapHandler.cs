using System;
using Bureaucratize.Common.Core.Infrastructure;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.FileStorage.Contracts.Queries;
using Flurl.Http;

namespace Bureaucratize.ImageProcessing.Infrastructure.ResourceQueryHandlers
{
    public class GetImageRecognitionLabelMapHandler : IResourceQueryHandler<GetImageRecognitionLabelMap, string>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;

        public GetImageRecognitionLabelMapHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null)
                throw new ArgumentNullException(nameof(persistenceConfiguration));
            _persistenceConfiguration = persistenceConfiguration;
        }

        public string Handle(GetImageRecognitionLabelMap command)
        {
            var url = _persistenceConfiguration.FileStorageApiImageRecognitionUrl +
                      $"/image-recognition-labelmap/{(byte)command.ExpectedData}";
            return url.GetStringAsync().Result;
        }
    }
}
