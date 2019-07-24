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
using Bureaucratize.Common.Core;
using Bureaucratize.Templating.Core.Template.Contracts;

namespace Bureaucratize.Templating.Core.ResultMessages.Details
{
    public class PageAreaNotInPageDefinition : IResultDetails
    {
        public string DetailsMessageKey => nameof(PageAreaNotInPageDefinition);

        public readonly Guid PagePartId;
        public readonly Guid PageDefinitionId;

        public PageAreaNotInPageDefinition(Guid templatePagePartId, ITemplatePageDefinition pageDefinition)
        {
            PagePartId = templatePagePartId;
            PageDefinitionId = pageDefinition.Id;
        }

        public string GetDetails()
        {
            return $"Page area of id {PagePartId} not found in page definition of id {PageDefinitionId}";
        }
    }
}
