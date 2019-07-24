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
using System.Drawing;
using System.Linq;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.Common.Core.Infrastructure.ResultMessages;
using Bureaucratize.Common.Core.Utils;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Commands;
using Bureaucratize.Templating.Core.Infrastructure.Commands;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels;
using Microsoft.EntityFrameworkCore;

namespace Bureaucratize.Templating.Infrastructure.NetStand.CommandHandlers
{
    public class AddTemplateDefinitionPageHandler : ICommandHandler<AddTemplateDefinitionPage, TemplateDefinition>
    {
        private readonly ITemplatingPersistenceConfiguration _persistenceConfiguration;
        private readonly IResourceCommandHandler<SaveBitmapForTemplatePageCanvasDefinition, FileStorageRequestResult> _saveCanvasHandler;

        public AddTemplateDefinitionPageHandler(ITemplatingPersistenceConfiguration persistenceConfiguration, 
            IResourceCommandHandler<SaveBitmapForTemplatePageCanvasDefinition, FileStorageRequestResult> saveCanvasHandler)
        {
            if (persistenceConfiguration == null) throw new ArgumentNullException(nameof(persistenceConfiguration));
            if (saveCanvasHandler == null) throw new ArgumentNullException(nameof(saveCanvasHandler));

            _persistenceConfiguration = persistenceConfiguration;
            _saveCanvasHandler = saveCanvasHandler;
        }

        public OperationResult<TemplateDefinition> Handle(AddTemplateDefinitionPage command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using (var context = new TemplatingContext(_persistenceConfiguration))
            {
                var templateFromDb = context.Templates
                        .Include(t => t.DefinedPages)
                    .SingleOrDefault(t => t.Id == command.TemplateId);

                var template = templateFromDb?.AsDomainModel();
                if (template == null)
                {
                    return OperationResult<TemplateDefinition>.Failure(
                        new ObjectNotFoundById(typeof(TemplateDefinition), command.TemplateId));
                }

                var templatePage = new TemplatePageDefinition((int)command.PageNumber);

                using (var bitmap = command.ReferenceCanvas.FileData.AsBitmap())
                {
                    var modificationResult = templatePage
                        .ModifyReferenceCanvas(new Rectangle(0, 0, bitmap.Width, bitmap.Height));

                    if (!modificationResult.Successful)
                    {
                        return OperationResult<TemplateDefinition>.Failure(modificationResult.Details);
                    }
                }

                var addPageResult = template.AddPageDefinition(templatePage);
                if (!addPageResult.Successful)
                {
                    return OperationResult<TemplateDefinition>.Failure(addPageResult.Details);
                }

                templateFromDb.DefinedPages.Add(templatePage.AsPersistenceModel());
                context.Update(templateFromDb);
                context.SaveChanges();

                var saveBitmapCommand = new SaveBitmapForTemplatePageCanvasDefinition
                {
                    FileData = command.ReferenceCanvas.FileData,
                    FileLabel = command.ReferenceCanvas.FileLabel,
                    FileType = command.ReferenceCanvas.FileType,
                    TemplatePageId = templatePage.Id,
                    TemplateId = template.Id
                };
                var savedCanvas = _saveCanvasHandler.Handle(saveBitmapCommand);

                if (!savedCanvas.Success)
                {
                    return OperationResult<TemplateDefinition>.Failure(new FileStorageSaveFailed(saveBitmapCommand));
                }

                return OperationResult<TemplateDefinition>.Success(template);
            }
        }
    }
}
