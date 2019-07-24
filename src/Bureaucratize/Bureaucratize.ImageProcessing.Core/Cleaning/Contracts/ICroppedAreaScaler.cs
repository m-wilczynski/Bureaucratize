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
using Bureaucratize.Common.Core.Structures;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.ImageProcessing.Core.Cropping.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Cleaning.Contracts
{
    public interface ICroppedAreaScaler
    {
        Dimension2D ExpectedSize2D { get; }
        ProcessingResult<FlattenedCroppedArea> ScaleAndFlatten(ICroppedArea croppedArea);

        Dimension2D GetInputSizeFromBitmap(Bitmap bitmap);
        Dimension2D GetDimensionsForResizeBeforeCentering(Dimension2D inputSizeAsMultiplesOfFour);
    }
}
