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
using System.Collections.Generic;
using System.Linq;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels.BaseModel;

namespace Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels
{
    internal class TemplatePersistenceModel : PersistenceModel
    {
        public TemplatePersistenceModel()
        {
            DefinedPages = new List<TemplatePagePersistenceModel>();
        }

        public string TemplateName { get; set; }
        public Guid TemplateCreatorId { get; set; }
        public ICollection<TemplatePagePersistenceModel> DefinedPages { get; set; }
    }

    internal static class TemplateExtensions
    {
        public static TemplateDefinition AsDomainModel(this TemplatePersistenceModel dbModel)
        {
            var template = new TemplateDefinition(dbModel.TemplateName, dbModel.TemplateCreatorId, dbModel.Id);

            foreach (var page in dbModel.DefinedPages.Select(p => p.AsDomainModel()))
            {
                template.AddPageDefinition(page);
            }

            return template;
        }

        public static TemplatePersistenceModel AsPersistenceModel(this TemplateDefinition domainModel)
        {
            return new TemplatePersistenceModel
            {
                Id = domainModel.Id,
                TemplateName = domainModel.TemplateName,
                TemplateCreatorId = domainModel.CreatorId,
                DefinedPages = domainModel.DefinedPages.Values
                    .OfType<TemplatePageDefinition>()
                    .Select(p => p.AsPersistenceModel())
                    .ToList()
            };
        }
    }
}
