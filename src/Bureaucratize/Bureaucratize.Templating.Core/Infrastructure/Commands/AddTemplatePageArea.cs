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
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.Templating.Core.Template;

namespace Bureaucratize.Templating.Core.Infrastructure.Commands
{
    public class AddTemplatePageArea : ICommand
    {
        public Guid TemplatePageId { get; set; }

        public int DimensionX { get; set; }
        public int DimensionY { get; set; }
        public int DimensionWidth { get; set; }
        public int DimensionHeight { get; set; }
        public string AreaName { get; set; }
        public TemplatePartExpectedDataType ExpectedData { get; set; }

        public AddTemplatePageAreaPart[] AreaParts { get; set; }
    }

    //TODO: Does it even make sense now?
    public class AddTemplatePageAreas : ICommand
    {
        public Guid TemplatePageId { get; set; }
        public AddTemplatePageArea[] Areas { get; set; }
    }

    public class AddTemplatePageAreaPart
    {
        public int DimensionX { get; set; }
        public int DimensionY { get; set; }
        public int DimensionWidth { get; set; }
        public int DimensionHeight { get; set; }
        public int OrderInArea { get; set; }
    }
}
