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
using Bureaucratize.ImageProcessing.Core.Extracting.Contracts;
using Bureaucratize.Templating.Core.Template;

namespace Bureaucratize.ImageProcessing.Core.Cleaning.Contracts
{
    /// <summary>
    /// Defines API for matching scanned input to defined template canvas (ie. fixing displacement from scanning/printing etc)
    /// </summary>
    public interface ITemplateCanvasMatcher
    {
        TemplatePageCanvasDefinition ReferenceCanvas { get; }

        /// <summary>
        /// Fixes any displacement, rotation etc. of <see cref="Bitmap"/> that could interfere with extraction using <see cref="IHandwrittenInputExtractor"/>
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        ProcessingResult<Bitmap> NormalizeToMatchTemplateCanvas(Bitmap userInput);
    }
}
