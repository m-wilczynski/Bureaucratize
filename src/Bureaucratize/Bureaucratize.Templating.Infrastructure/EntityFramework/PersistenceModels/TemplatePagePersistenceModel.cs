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
using System.Linq;
using Bureaucratize.Templating.Core.InterestPoints;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure.EntityFramework.PersistenceModels.BaseModel;

namespace Bureaucratize.Templating.Infrastructure.EntityFramework.PersistenceModels
{
    internal class TemplatePagePersistenceModel : PersistenceModel
    {
        public TemplatePagePersistenceModel()
        {
            DefinedAreas = new List<TemplatePageAreaPersistenceModel>();
        }

        public int PageNumber { get; set; }
        public TemplatePageCanvasPersistenceModel ReferenceCanvas { get; set; }
        public ICollection<TemplatePageAreaPersistenceModel> DefinedAreas { get; set; }
    }

    internal static class TemplatePagePersistenceExtensions
    {
        public static TemplatePageDefinition AsDomainModel(this TemplatePagePersistenceModel dbModel)
        {
            var page = new TemplatePageDefinition(dbModel.PageNumber, dbModel.Id);
            page.ModifyReferenceCanvas(dbModel.ReferenceCanvas.AsDomainModel().CanvasDimensions);

            foreach (var pageArea in dbModel.DefinedAreas.Select(da => da.AsDomainModel()))
            {
                page.DefineArea(pageArea);
            }

            return page;
        }

        public static TemplatePagePersistenceModel AsPersistenceModel(this TemplatePageDefinition domainModel)
        {
            return new TemplatePagePersistenceModel
            {
                Id = domainModel.Id,
                PageNumber = domainModel.PageNumber,
                ReferenceCanvas = domainModel.ReferenceCanvas.AsPersistenceModel(),
                DefinedAreas = domainModel.DefinedAreas
                                                .OfType<TemplatePageArea>()
                                                .Select(da => da.AsPersistenceModel())
                                                .ToList()
            };
        }
    }
}
