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
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.Templating.Core.Template.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Extracting.Contracts
{
    /// <summary>
    /// Defines API for extracting handwritten input from scanned <see cref="Bitmap"/>
    /// </summary>
    public interface IHandwrittenInputExtractor
    {
        /// <summary>
        /// Extracts <see cref="Bitmap"/> with only hand writing on it, ie. removes printed parts
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="templatePage"></param>
        /// <returns></returns>
        ProcessingResult<Bitmap> ExtractHandwrittenInput(Bitmap userInput, ITemplatePageDefinition templatePage);
    }
}
