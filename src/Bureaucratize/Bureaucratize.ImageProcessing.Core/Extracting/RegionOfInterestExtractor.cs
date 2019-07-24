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
using System.Drawing.Imaging;
using Accord.Imaging.Filters;
using Bureaucratize.Common.Core.Structures;
using Bureaucratize.ImageProcessing.Contracts.Bitmaps;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.ImageProcessing.Core.Common;
using Bureaucratize.ImageProcessing.Core.Cropping;
using Bureaucratize.ImageProcessing.Core.Cropping.Contracts;
using Bureaucratize.ImageProcessing.Core.Extracting.Contracts;
using Bureaucratize.ImageProcessing.Core.Utils;

namespace Bureaucratize.ImageProcessing.Core.Extracting
{
    public class RegionOfInterestExtractor : IRegionOfInterestExtractor
    {
        public ProcessingResult<ICroppedArea> ExtractRegionOfInterestFrom(ICroppedArea areaOfExtraction)
        {
            using (areaOfExtraction)
            {
                List<OrderedBitmap> extractedRegionsOfInterest = new List<OrderedBitmap>();

                foreach (var areaPart in areaOfExtraction.CroppedParts)
                {
                    var partBytes = areaPart.Bitmap.AsFlattenedByteArray();

                    var leftEdge = GetLeftEdgeOfRegionOfInterest(partBytes);
                    var rightEdge = GetRightEdgeOfRegionOfInterest(partBytes);
                    var topEdge = GetTopEdgeOfRegionOfInterest(partBytes);                    
                    var bottomEdge = GetBottomEdgeOfRegionOfInterest(partBytes);

                    if (leftEdge == null || rightEdge == null || topEdge == null || bottomEdge == null
                        || (bottomEdge.Value - topEdge.Value) < 10)
                    {
                        //Nothing interesting - return empty result
                        extractedRegionsOfInterest.Add(new OrderedBitmap(areaPart.Order, null));
                        continue;
                    }

                    var size = new Dimension2D(rightEdge.Value - leftEdge.Value + 1, bottomEdge.Value - topEdge.Value + 1)
                        .GetInputSizeAsMultipliesOfFour().AsSize();

                    var croppedBitmap = new Crop(new Rectangle(new Point(leftEdge.Value, topEdge.Value), size)).Apply(areaPart.Bitmap);

                    extractedRegionsOfInterest.Add(new OrderedBitmap(areaPart.Order, croppedBitmap));
                }

                return ProcessingResult<ICroppedArea>.Success(
                    new CroppedArea(areaOfExtraction.AreaUsedForCropping, extractedRegionsOfInterest, areaOfExtraction.DocumentId));
            }
        }

        private static int? GetTopEdgeOfRegionOfInterest(FlattenedBitmap flattenedBitmap)
        {
            for (int i = 0; i < flattenedBitmap.Height - 1; ++i)
            {
                for (int j = 0; j < flattenedBitmap.Stride - 1; ++j)
                {
                    if (flattenedBitmap.Data[i, j] < 255)
                    {
                        return i == 0 ? 0 : i - 1;
                    }
                }
            }

            return null;
        }

        private static int? GetBottomEdgeOfRegionOfInterest(FlattenedBitmap flattenedBitmap)
        {
            for (int i = flattenedBitmap.Height - 1; i >= 0; --i)
            {
                for (int j = flattenedBitmap.Stride - 1; j >= 0; --j)
                {
                    if (flattenedBitmap.Data[i, j] < 255)
                    {
                        return i == flattenedBitmap.Height - 1 ? i : i + 1;
                    }
                }
            }

            return null;
        }

        private static int? GetLeftEdgeOfRegionOfInterest(FlattenedBitmap flattenedBitmap)
        {
            for (int i = 0; i < flattenedBitmap.Stride - 1; ++i)
            {
                for (int j = 0; j < flattenedBitmap.Height - 1; ++j)
                {
                    if (flattenedBitmap.Data[j, i] < 255)
                    {
                        return i <= 0 ? i : i - 1;
                    }
                }
            }

            return null;
        }

        private static int? GetRightEdgeOfRegionOfInterest(FlattenedBitmap flattenedBitmap)
        {
            for (int i = flattenedBitmap.Stride - 1; i >= 0; --i)
            {
                for (int j = flattenedBitmap.Height - 1; j >= 0; --j)
                {
                    if (flattenedBitmap.Data[j, i] < 255)
                    {
                        return i >= flattenedBitmap.Stride - 1 ? i : i + 1;
                    }
                }
            }

            return null;
        }
    }
}
