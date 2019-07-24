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
using Bureaucratize.Templating.Infrastructure;
using Bureaucratize.Templating.Infrastructure.NetStand;
using Microsoft.Extensions.Configuration;

namespace Bureaucratize.Web.Config
{
    public class TemplatingPersistenceConfiguration : ITemplatingPersistenceConfiguration
    {
        private readonly IConfiguration _configuration;

        public TemplatingPersistenceConfiguration(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
        }

        public string DatabaseConnectionString => _configuration.GetConnectionString("TemplatingDb");

        public string FileStorageApiUrl => _configuration["FileStorageService:ServiceAddress"] + "/api";

        public string FileStorageApiImageRecognitionUrl =>
            FileStorageApiUrl + "/image-recognition-trained-files/";

        public string FileStorageApiProcessingStepsUrl =>
            FileStorageApiUrl + "/processing-step-images/";

        public string FileStorageApiTemplateFilesUrl =>
            FileStorageApiUrl + "/template-files/";

        public string FileStorageApiUserDocumentsUrl =>
            FileStorageApiUrl + "/user-documents/";
    }
}
