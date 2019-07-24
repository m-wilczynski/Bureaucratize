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

using System.Drawing;
using Bureaucratize.Templating.Core.InterestPoints;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels.BaseModel;

namespace Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels
{
    internal class TemplatePageAreaPartPersistenceModel : PersistenceModel
    {
        public int DimensionX { get; set; }
        public int DimensionY { get; set; }
        public int DimensionWidth { get; set; }
        public int DimensionHeight { get; set; }
        public int OrderInArea { get; set; }
    }

    internal static class TemplatePageAreaPartExtensions
    {
        public static TemplatePageAreaPart AsDomainModel(this TemplatePageAreaPartPersistenceModel dbModel)
        {
            return new TemplatePageAreaPart(
                new Rectangle(dbModel.DimensionX, dbModel.DimensionY, dbModel.DimensionWidth, dbModel.DimensionHeight), 
                (uint)dbModel.OrderInArea, dbModel.Id);
        }

        public static TemplatePageAreaPartPersistenceModel AsPersistenceModel(this TemplatePageAreaPart domainModel)
        {
            return new TemplatePageAreaPartPersistenceModel
            {
                Id = domainModel.Id,
                OrderInArea = (int)domainModel.OrderInArea,
                DimensionX = domainModel.Dimension.X,
                DimensionY = domainModel.Dimension.Y,
                DimensionWidth = domainModel.Dimension.Width,
                DimensionHeight = domainModel.Dimension.Height
            };
        }
    }
}
