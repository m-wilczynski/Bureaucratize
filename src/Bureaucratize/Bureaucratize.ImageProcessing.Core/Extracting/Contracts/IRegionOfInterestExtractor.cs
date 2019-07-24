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

using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.ImageProcessing.Core.Cropping.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Extracting.Contracts
{
    /// <summary>
    /// Defines API for extracting region of interest, ie. area cropped with only interesting data
    /// </summary>
    public interface IRegionOfInterestExtractor
    {
        /// <summary>
        /// Extracts region of interest, ie. area's bitmap cropped with only interesting data
        /// </summary>
        /// <param name="areaOfExtraction"></param>
        /// <returns></returns>
        ProcessingResult<ICroppedArea> ExtractRegionOfInterestFrom(ICroppedArea areaOfExtraction);
    }
}
