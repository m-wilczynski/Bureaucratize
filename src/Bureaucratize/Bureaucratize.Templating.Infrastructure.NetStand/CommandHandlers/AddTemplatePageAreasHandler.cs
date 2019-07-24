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
using Bureaucratize.Common.Core.Infrastructure.ResultMessages;
using Bureaucratize.Templating.Core.Infrastructure.Commands;
using Bureaucratize.Templating.Core.InterestPoints;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels;
using Microsoft.EntityFrameworkCore;

namespace Bureaucratize.Templating.Infrastructure.NetStand.CommandHandlers
{
    public class AddTemplatePageAreasHandler : ICommandHandler<AddTemplatePageAreas, TemplatePageDefinition>
    {
        private readonly ITemplatingPersistenceConfiguration _persistenceConfiguration;

        public AddTemplatePageAreasHandler(ITemplatingPersistenceConfiguration persistenceConfiguration)
        {
            if (persistenceConfiguration == null) throw new ArgumentNullException(nameof(persistenceConfiguration));
            _persistenceConfiguration = persistenceConfiguration;
        }

        public OperationResult<TemplatePageDefinition> Handle(AddTemplatePageAreas command)
        {
            ThrowOnInvalidInput(command);

            using (var context = new TemplatingContext(_persistenceConfiguration))
            {
                var pageFromDb = context.Pages
                        .Include(p => p.ReferenceCanvas)
                        .Include(p => p.DefinedAreas)
                    .SingleOrDefault(p => p.Id == command.TemplatePageId);
                var page = pageFromDb?.AsDomainModel();

                if (page == null)
                {
                    return OperationResult<TemplatePageDefinition>.Failure(
                        new ObjectNotFoundById(typeof(TemplatePageDefinition), command.TemplatePageId));
                }

                foreach (var area in command.Areas)
                {
                    var newArea = new TemplatePageArea(
                        new Rectangle(area.DimensionX, area.DimensionY, area.DimensionWidth, area.DimensionHeight),
                        area.AreaName, area.ExpectedData);

                    foreach (var areaPart in area.AreaParts)
                    {
                        var addAreaPartResult = newArea.DefinePointOfInterest(
                            new Rectangle(areaPart.DimensionX, areaPart.DimensionY, areaPart.DimensionWidth, areaPart.DimensionHeight),
                            (uint)areaPart.OrderInArea);

                        if (!addAreaPartResult.Successful)
                        {
                            return OperationResult<TemplatePageDefinition>.Failure(addAreaPartResult.Details);
                        }
                    }

                    var addPageResult = page.DefineArea(newArea);

                    if (!addPageResult.Successful)
                    {
                        return OperationResult<TemplatePageDefinition>.Failure(addPageResult.Details);
                    }

                    pageFromDb.DefinedAreas.Add(newArea.AsPersistenceModel());
                }

                context.Update(pageFromDb);
                context.SaveChanges();

                return OperationResult<TemplatePageDefinition>.Success(page);
            }
        }

        private static void ThrowOnInvalidInput(AddTemplatePageAreas command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.Areas == null)
            {
                throw new ArgumentNullException(nameof(command.Areas));
            }
        }
    }
}
