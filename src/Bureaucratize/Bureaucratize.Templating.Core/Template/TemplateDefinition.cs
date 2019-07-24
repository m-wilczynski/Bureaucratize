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
using Bureaucratize.Common.Core;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Templating.Core.ResultMessages;
using Bureaucratize.Templating.Core.ResultMessages.Details;
using Bureaucratize.Templating.Core.Template.Contracts;

namespace Bureaucratize.Templating.Core.Template
{
    public class TemplateDefinition : Identifiable, ITemplateDefinition
    {
        private readonly Guid _templateCreatorId;
        private readonly string _templateName;
        private readonly Dictionary<int, ITemplatePageDefinition> _definedPages = new Dictionary<int, ITemplatePageDefinition>();

        public TemplateDefinition(string templateName, Guid templateCreatorId, Guid? id = null)
            : base(id)
        {
            if (templateName == null)
                throw new ArgumentNullException(nameof(templateName));
            if (templateCreatorId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(templateCreatorId));

            _templateName = templateName;
            _templateCreatorId = templateCreatorId;
        }

        public Guid CreatorId => _templateCreatorId;
        public string TemplateName => _templateName;
        public bool IsCompleteDefinition => DefinedPages.Values.Count(dp => dp.IsCompleteDefinition) > 0;
        public IReadOnlyDictionary<int, ITemplatePageDefinition> DefinedPages => _definedPages;

        public TemplateModificationResult AddEmptyPageDefinition(int pageNumber)
        {
            if (pageNumber < 1)
                return TemplateModificationResult.Failure(new NonPositivePageNumber(pageNumber));
            return AddPageDefinition(new TemplatePageDefinition(pageNumber));
        }

        public TemplateModificationResult AddPageDefinition(ITemplatePageDefinition pageDefinition)
        {
            if (pageDefinition == null)
                return TemplateModificationResult.Failure(new EmptyInput());
            if (_definedPages.ContainsKey(pageDefinition.PageNumber))
                return TemplateModificationResult.Failure(new DuplicatedPageNumber(pageDefinition.PageNumber));

            _definedPages.Add(pageDefinition.PageNumber, pageDefinition);
            return TemplateModificationResult.Success();
        }

        //TODO: Should be even possible? Will break all of the DocumentToProcess
        public TemplateModificationResult RearrangePages()
        {
            //TODO: Rearrange pages in template so there are no gaps etc.
            throw new NotImplementedException();
        }
    }
}
