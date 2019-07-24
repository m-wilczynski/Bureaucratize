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
using System.Linq;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Common.Core.Infrastructure.ResultMessages;
using Bureaucratize.Templating.Core.Infrastructure.Queries;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels;
using Microsoft.EntityFrameworkCore;

namespace Bureaucratize.Templating.Infrastructure.NetStand.QueryHandlers
{
    public class GetTemplateDefinitionByIdHandler : IQueryHandler<GetTemplateDefinitionById, TemplateDefinition>
    {
        private readonly ITemplatingPersistenceConfiguration _persistenceConfiguration;

        public GetTemplateDefinitionByIdHandler(ITemplatingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null)
                throw new ArgumentNullException(nameof(persistenceConfiguration));

            _persistenceConfiguration = persistenceConfiguration;
        }

        public OperationResult<TemplateDefinition> Handle(GetTemplateDefinitionById query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            using (var context = new TemplatingContext(_persistenceConfiguration))
            {
                try
                {
                    return OperationResult<TemplateDefinition>.Success(context.Templates
                            .Include(t => t.DefinedPages)
                                .ThenInclude(dp => dp.ReferenceCanvas)
                            .Include(t => t.DefinedPages)
                                .ThenInclude(dp => dp.DefinedAreas)
                                .ThenInclude(da => da.InterestPoints)
                        .SingleOrDefault(t => t.Id == query.TemplateId)?.AsDomainModel());
                }
                catch (Exception ex)
                {
                    return OperationResult<TemplateDefinition>.Failure(new UncaughtException(ex));
                }
            }
        }
    }
}
