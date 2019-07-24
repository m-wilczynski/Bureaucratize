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
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.Templating.Core.Template.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Cropping.Contracts
{
    /// <summary>
    /// Defines API for cropping bitmap, based on points of interest (<see cref="ITemplatePageArea"/>) from <see cref="ITemplatePageDefinition"/>
    /// </summary>
    public interface ITemplateAreasCropper
    {
        /// <summary>
        /// Crops <see cref="Bitmap"/> into collection of <see cref="ICroppedArea"/>, based on definition provided by <see cref="ITemplatePageDefinition"/>
        /// </summary>
        /// <param name="bitmapToCropIntoParts"></param>
        /// <returns></returns>
        ProcessingResult<ICollection<ICroppedArea>> CropUserInput(Bitmap bitmapToCropIntoParts, Guid documentId,
            ITemplatePageDefinition definitionForCropping);
    }
}
