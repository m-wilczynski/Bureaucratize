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
using Bureaucratize.Common.Core.Infrastructure;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.FileStorage.Contracts.Queries;
using Flurl.Http;

namespace Bureaucratize.ImageProcessing.Infrastructure.ResourceQueryHandlers
{
    public class GetImageRecognitionModelHandler : IResourceQueryHandler<GetImageRecognitionModel, byte[]>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;

        public GetImageRecognitionModelHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null)
                throw new ArgumentNullException(nameof(persistenceConfiguration));
            _persistenceConfiguration = persistenceConfiguration;
        }

        public byte[] Handle(GetImageRecognitionModel command)
        {
            var url = _persistenceConfiguration.FileStorageApiImageRecognitionUrl + 
                      $"/image-recognition-model/{(byte)command.ExpectedData}";
            return url.GetBytesAsync().Result;
        }
    }
}
