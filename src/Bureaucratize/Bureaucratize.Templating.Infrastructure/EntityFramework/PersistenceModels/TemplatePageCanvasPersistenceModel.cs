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
using Bureaucratize.Templating.Core.Template;
using Bureaucratize.Templating.Infrastructure.EntityFramework.PersistenceModels.BaseModel;

namespace Bureaucratize.Templating.Infrastructure.EntityFramework.PersistenceModels
{
    internal class TemplatePageCanvasPersistenceModel : PersistenceModel
    {
        public int DimensionX { get; set; }
        public int DimensionY { get; set; }
        public int DimensionWidth { get; set; }
        public int DimensionHeight { get; set; }
    }

    internal static class TemplatePageCanvasExtensions
    {
        public static TemplatePageCanvasDefinition AsDomainModel(this TemplatePageCanvasPersistenceModel dbModel)
        {
            return new TemplatePageCanvasDefinition(
                new Rectangle(dbModel.DimensionX, dbModel.DimensionY, dbModel.DimensionWidth, dbModel.DimensionHeight),
                dbModel.Id);
        }

        public static TemplatePageCanvasPersistenceModel AsPersistenceModel(this TemplatePageCanvasDefinition domainModel)
        {
            return new TemplatePageCanvasPersistenceModel
            {
                Id = domainModel.Id,
                DimensionX = domainModel.CanvasDimensions.X,
                DimensionY = domainModel.CanvasDimensions.Y,
                DimensionWidth = domainModel.CanvasDimensions.Width,
                DimensionHeight = domainModel.CanvasDimensions.Height
            };
        }
    }
}
