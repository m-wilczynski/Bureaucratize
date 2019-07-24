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
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Common.Core.Infrastructure.ResultMessages;
using Bureaucratize.Templating.Core.Infrastructure.Commands;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels;

namespace Bureaucratize.Templating.Infrastructure.NetStand.CommandHandlers
{
    public class CreateTemplateDefinitionHandler : ICommandHandler<CreateTemplateDefinition, TemplateDefinition>
    {
        private readonly ITemplatingPersistenceConfiguration _persistenceConfiguration;

        public CreateTemplateDefinitionHandler(ITemplatingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null) throw new ArgumentNullException(nameof(persistenceConfiguration));
            _persistenceConfiguration = persistenceConfiguration;
        }

        public OperationResult<TemplateDefinition> Handle(CreateTemplateDefinition command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using (var context = new TemplatingContext(_persistenceConfiguration))
            {
                try
                {
                    var definition = new TemplateDefinition(command.TemplateName, command.TemplateCreatorId);
                    context.Templates.Add(definition.AsPersistenceModel());

                    context.SaveChanges();
                    return OperationResult<TemplateDefinition>.Success(definition);
                }
                catch (Exception ex)
                {
                    return OperationResult<TemplateDefinition>.Failure(new UncaughtException(ex));
                }
            }
        }
    }
}
