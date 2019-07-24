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

using System.Collections.Generic;
using Autofac;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Common.Core.Infrastructure.Common.Shortcuts;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Models;
using Bureaucratize.FileStorage.Contracts.Queries;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes.Models;
using Bureaucratize.ImageProcessing.Contracts.Recognition;
using Bureaucratize.ImageProcessing.Core.Document;
using Bureaucratize.ImageProcessing.Core.Queries;
using Bureaucratize.ImageProcessing.Core.Recognition;
using Bureaucratize.ImageProcessing.Host.Configuration;
using Bureaucratize.ImageProcessing.Infrastructure;
using Bureaucratize.ImageProcessing.Infrastructure.CommandHandlers;
using Bureaucratize.ImageProcessing.Infrastructure.QueryHandlers;
using Bureaucratize.ImageProcessing.Infrastructure.ResourceQueryHandlers;
using Bureaucratize.Templating.Core.Infrastructure.Queries;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure;
using Bureaucratize.Templating.Infrastructure.QueryHandlers;

namespace Bureaucratize.ImageProcessing.Host.IoC.Modules
{
    public class PersistenceDependenciesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ImageProcessingPersistenceConfiguration>()
                   .As<IImageProcessingPersistenceConfiguration>();

            builder.RegisterType<TemplatingPersistenceConfiguration>()
                .As<ITemplatingPersistenceConfiguration>();

            RegisterImageProcessingHandlers(builder);
        }

        private static void RegisterImageProcessingHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<GetDocumentToProcessHandler>()
                   .As<IQueryHandler<GetDocumentToProcess, DocumentToProcess>>();

            builder.RegisterType<GetDocumentToProcessResourcesHandler>()
                   .As<IResourceQueryHandler<GetBitmapsForDocumentToProcess,FileStorageRequestResult<ICollection<OrderedBitmapResource>>>>();

            builder.RegisterType<GetTemplateDefinitionByIdHandler>()
                   .As<IQueryHandler<GetTemplateDefinitionById, TemplateDefinition>>();

            builder.RegisterType<SaveRecognizedTextPartHandler>()
                .As<ICommandHandler<SaveDomainObject<RecognizedTextPart>>>();

            builder.RegisterType<SaveRecognizedChoicePartHandler>()
                .As<ICommandHandler<SaveDomainObject<RecognizedChoicePart>>>();

            builder.RegisterType<SaveProcessedDocumentPageHandler>()
                .As<ICommandHandler<SaveDomainObject<ProcessedDocumentPage>>>();

            builder.RegisterType<GetCanvasBitmapForTemplatePageResourceHandler>()
                .As<IResourceQueryHandler<GetCanvasBitmapForTemplatePage, TemplatePageCanvasBitmapResource>>();

            builder.RegisterType<GetImageRecognitionLabelMapHandler>()
                .As<IResourceQueryHandler<GetImageRecognitionLabelMap, string>>();

            builder.RegisterType<GetImageRecognitionModelHandler>()
                .As<IResourceQueryHandler<GetImageRecognitionModel, byte[]>>();

        }
    }
}
