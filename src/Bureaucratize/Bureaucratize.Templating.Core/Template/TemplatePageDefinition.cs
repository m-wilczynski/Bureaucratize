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
using System.Drawing;
using Bureaucratize.Common.Core;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.Templating.Core.InterestPoints.Contracts;
using Bureaucratize.Templating.Core.ResultMessages;
using Bureaucratize.Templating.Core.ResultMessages.Details;
using Bureaucratize.Templating.Core.Template.Contracts;

namespace Bureaucratize.Templating.Core.Template
{
    public class TemplatePageDefinition : Identifiable, ITemplatePageDefinition
    {
        private readonly Dictionary<string, ITemplatePageArea> _definedAreas = new Dictionary<string, ITemplatePageArea>();
        private TemplatePageCanvasDefinition _referenceCanvas;
        private int _pageNumber;

        public TemplatePageDefinition(int pageNumber, Guid? id = null)
            : base(id)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            _pageNumber = pageNumber;
        }

        public TemplatePageCanvasDefinition ReferenceCanvas => _referenceCanvas;
        public IReadOnlyCollection<ITemplatePageArea> DefinedAreas => _definedAreas.Values;
        public bool IsCompleteDefinition => _definedAreas.Values.Count > 0 && _referenceCanvas != null;
        public int PageNumber => _pageNumber;

        public TemplateModificationResult DefineArea(ITemplatePageArea area)
        {
            if (area == null)
            {
                return TemplateModificationResult.Failure(new EmptyInput());
            }
            if (_definedAreas.ContainsKey(area.AreaName))
            {
                //TODO: DuplicatedPageAreaName
                return TemplateModificationResult.Failure(null);
            }

            _definedAreas.Add(area.AreaName, area);
            return TemplateModificationResult.Success();
        }

        public TemplateModificationResult<TemplatePageCanvasDefinition> ModifyReferenceCanvas(Rectangle? newDimensions = null)
        {
            if (newDimensions == null)
                return TemplateModificationResult<TemplatePageCanvasDefinition>.Failure(new EmptyInput());

            if (_referenceCanvas == null)
            {
                _referenceCanvas = new TemplatePageCanvasDefinition(newDimensions.Value);
            }
            else
            {
                _referenceCanvas = new TemplatePageCanvasDefinition(
                    newDimensions ?? _referenceCanvas.CanvasDimensions);
            }

            return TemplateModificationResult<TemplatePageCanvasDefinition>.Success(ReferenceCanvas);
        }
    }
}
