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
using Bureaucratize.Templating.Core.InterestPoints.Contracts;
using Bureaucratize.Templating.Core.ResultMessages;
using Bureaucratize.Templating.Core.ResultMessages.Details;
using Bureaucratize.Templating.Core.Template;

namespace Bureaucratize.Templating.Core.InterestPoints
{
    public class TemplatePageArea : Identifiable, ITemplatePageArea
    {
        private readonly HashSet<TemplatePageAreaPart> _interestPoints = new HashSet<TemplatePageAreaPart>();

        public TemplatePageArea(Rectangle definedDimension, string areaName, TemplatePartExpectedDataType expectedData, Guid? id = null)
            : base(id)
        {
            if (string.IsNullOrWhiteSpace(areaName))
            {
                throw new ArgumentNullException(nameof(areaName));
            }

            AreaDimension = definedDimension;
            AreaName = areaName;
            ExpectedData = expectedData;
        }

        public Rectangle AreaDimension { get; }
        public string AreaName { get; }
        public TemplatePartExpectedDataType ExpectedData { get; }
        public IReadOnlyCollection<TemplatePageAreaPart> InterestPoints => _interestPoints;

        public override bool Equals(object obj)
        {
            if (!(obj is TemplatePageArea)) return false;
            return ((TemplatePageArea) obj).Equals(this);
        }

        public bool Equals(TemplatePageArea other)
        {
            return AreaDimension.Equals(other.AreaDimension);
        }

        public override int GetHashCode()
        {
            return AreaDimension.GetHashCode();
        }

        public TemplateModificationResult<TemplatePageAreaPart> DefinePointOfInterest(Rectangle dimension, uint orderInArea)
        {
            if (!AreaDimension.Contains(dimension))
                return TemplateModificationResult<TemplatePageAreaPart>.Failure(new PageAreaPartNotInPageAreaDimension(dimension, AreaDimension));

            foreach (var interestPoint in _interestPoints)
            {
                if (interestPoint.Dimension == dimension)
                    return TemplateModificationResult<TemplatePageAreaPart>.Failure(new DuplicatedPageAreaDimension(dimension));
                if (interestPoint.OrderInArea == orderInArea)
                    return TemplateModificationResult<TemplatePageAreaPart>.Failure(new DuplicatedPageAreaPartOrder(orderInArea));
            }

            var areaPart = new TemplatePageAreaPart(dimension, orderInArea);
            _interestPoints.Add(areaPart);

            return TemplateModificationResult<TemplatePageAreaPart>.Success(areaPart);
        }
    }
}
