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
using System.IO;
using Bureaucratize.FileStorage.Contracts.Queries;
using Microsoft.Extensions.Options;

namespace Bureaucratize.FileStorage.Infrastructure.QueryHandlers
{
    public class GetImageRecognitionModelHandler : IFileStorageQueryHandler<GetImageRecognitionModel, byte[]>
    {
        private readonly IOptions<PersistenceConfiguration> _configuration;

        public GetImageRecognitionModelHandler(IOptions<PersistenceConfiguration> configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
        }

        public byte[] Handle(GetImageRecognitionModel query)
        {
            return File.ReadAllBytes(Path.Combine(
                _configuration.Value.ImageRecognitionPaths.RecognitionModelsPath, query.ExpectedData + ".Model"));
        }
    }
}
