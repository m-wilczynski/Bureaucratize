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
using System.Drawing;
using System.Linq;
using Bureaucratize.Templating.Core.InterestPoints;
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels.BaseModel;

namespace Bureaucratize.Templating.Infrastructure.NetStand.EntityFramework.PersistenceModels
{
    internal class TemplatePageAreaPersistenceModel : PersistenceModel
    {
        public int DimensionX { get; set; }
        public int DimensionY { get; set; }
        public int DimensionWidth { get; set; }
        public int DimensionHeight { get; set; }
        public string AreaName { get; set; }
        public int ExpectedData { get; set; }
        public ICollection<TemplatePageAreaPartPersistenceModel> InterestPoints { get; set; }
    }

    internal static class TemplatePageAreaExtensions
    {
        public static TemplatePageArea AsDomainModel(this TemplatePageAreaPersistenceModel dbModel)
        {
            var area = new TemplatePageArea(
                new Rectangle(dbModel.DimensionX, dbModel.DimensionY, dbModel.DimensionWidth, dbModel.DimensionHeight),
                dbModel.AreaName, (TemplatePartExpectedDataType)dbModel.ExpectedData, dbModel.Id);

            foreach (var areaPart in dbModel.InterestPoints.Select(part => part.AsDomainModel()))
            {
                area.DefinePointOfInterest(areaPart.Dimension, areaPart.OrderInArea);
            }

            return area;
        }

        public static TemplatePageAreaPersistenceModel AsPersistenceModel(this TemplatePageArea domainModel)
        {
            return new TemplatePageAreaPersistenceModel
            {
                Id = domainModel.Id,
                AreaName = domainModel.AreaName,
                DimensionX = domainModel.AreaDimension.X,
                DimensionY = domainModel.AreaDimension.Y,
                DimensionWidth = domainModel.AreaDimension.Width,
                DimensionHeight = domainModel.AreaDimension.Height,
                ExpectedData = (int)domainModel.ExpectedData,
                InterestPoints = domainModel.InterestPoints.Select(ip => ip.AsPersistenceModel()).ToList()
            };
        }
    }
}
