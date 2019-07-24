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
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Commands;
using Flurl.Http;

namespace Bureaucratize.ImageProcessing.Infrastructure.ResourceCommandHandlers
{
    public class SaveDocumentToProcessResourcesHandler : IResourceCommandHandler<SaveBitmapsForDocumentToProcess, FileStorageRequestResult>
    {
        private readonly IImageProcessingPersistenceConfiguration _persistenceConfiguration;

        public SaveDocumentToProcessResourcesHandler(IImageProcessingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null) throw new ArgumentNullException(nameof(persistenceConfiguration));

            _persistenceConfiguration = persistenceConfiguration;
        }

        public FileStorageRequestResult Handle(SaveBitmapsForDocumentToProcess command)
        {
            var result = $"{_persistenceConfiguration.FileStorageApiUserDocumentsUrl}/document-to-process/"
                .PostJsonAsync(command).ReceiveJson<FileStorageRequestResult>().Result;

            return result;
        }
    }
}
