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
using Accord.Imaging;
using Accord.Imaging.Filters;
using Bureaucratize.Common.Core.CommonDetails;
using Bureaucratize.ImageProcessing.Contracts.Bitmaps;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.Details;
using Bureaucratize.ImageProcessing.Core.Cleaning.Contracts;
using Bureaucratize.ImageProcessing.Core.Common;
using Bureaucratize.ImageProcessing.Core.Cropping;
using Bureaucratize.ImageProcessing.Core.Cropping.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Cleaning
{
    public class BradleyLocalThresholdingCleaner : ICroppedAreaCleaner
    {
        public ProcessingResult<ICroppedArea> CleanUp(ICroppedArea areaToClean)
        {
            if (areaToClean.CroppedParts == null || areaToClean.CroppedParts.Count == 0)
            {
                return ProcessingResult<ICroppedArea>.Failure(new EmptyInput());
            }

            using (areaToClean)
            {
                try
                {
                    var cleanedParts = new List<OrderedBitmap>();

                    foreach (var croppedPart in areaToClean.CroppedParts)
                    {
                        Bitmap formattedImage = Grayscale.CommonAlgorithms.RMY.Apply(croppedPart.Bitmap);
                        new BradleyLocalThresholding().ApplyInPlace(formattedImage);
                        cleanedParts.Add(new OrderedBitmap(croppedPart.Order, formattedImage));
                    }
                    return ProcessingResult<ICroppedArea>.Success(
                        new CroppedArea(areaToClean.AreaUsedForCropping, cleanedParts, areaToClean.DocumentId));
                }
                catch (UnsupportedImageFormatException formatException)
                {
                    return ProcessingResult<ICroppedArea>.Failure(new UnsupportedImageFormat());
                }
                catch (Exception generalException)
                {
                    return ProcessingResult<ICroppedArea>.Failure(new UncaughtException(generalException));
                }
            }
        }
    }
}
