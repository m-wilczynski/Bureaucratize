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
using Bureaucratize.Common.Core;

namespace Bureaucratize.Templating.Core.ResultMessages.Details
{
    public class PageAreaPartNotInPageAreaDimension : IResultDetails
    {
        public Rectangle AttemptedDimension { get; }
        public Rectangle AreaDimension { get; }
        public string DetailsMessageKey => nameof(PageAreaPartNotInPageAreaDimension);

        public PageAreaPartNotInPageAreaDimension(Rectangle attemptedDimension, Rectangle areaDimension)
        {
            AttemptedDimension = attemptedDimension;
            AreaDimension = areaDimension;
        }

        public string GetDetails()
        {
            return $"Attempted to define area part of {AttemptedDimension} " +
                   $"inside area of {AreaDimension} that would not fit.";
        }
    }
}
