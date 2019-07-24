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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Accord.Imaging.Filters;
using System.Drawing;
using Bureaucratize.Common.Core.Structures;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.ImageProcessing.Core.Cleaning.Contracts;
using Bureaucratize.ImageProcessing.Core.Common;
using Bureaucratize.ImageProcessing.Core.Cropping.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Cleaning
{

    public class EmnistCroppedAreaScaler : ICroppedAreaScaler
    {
        public Dimension2D ExpectedSize2D => new Dimension2D(28, 28);

        public ProcessingResult<FlattenedCroppedArea> ScaleAndFlatten(ICroppedArea croppedArea)
        {
            FlattenedCroppedArea output;
            using (croppedArea)
            {
                var flattenedResults = new List<OrderedFlattenedBitmap>();


                foreach (var croppedAreaPart in croppedArea.CroppedParts)
                {
                    if (croppedAreaPart.Bitmap == null)
                    {
                        flattenedResults.Add(new OrderedFlattenedBitmap(croppedAreaPart.Order, null));
                        continue;
                    }

                    //EMNIST expects black background and white text
                    new Invert().ApplyInPlace(croppedAreaPart.Bitmap);

                    //Bitmap will force stride to be 4x, so let's avoid mess up and do it ourselves
                    var inputSizeAsMultipliesOfFour = GetInputSizeFromBitmap(croppedAreaPart.Bitmap).GetInputSizeAsMultipliesOfFour();

                    //Downscale to height = 28 and width scaled by same factor
                    var inputSizeAfterDownscale = GetDimensionsForResizeBeforeCentering(inputSizeAsMultipliesOfFour);

                    //And fix it to mulitple of 4 again
                    inputSizeAfterDownscale = inputSizeAfterDownscale.GetInputSizeAsMultipliesOfFour();

                    //Blur it a little for better feature recognition
                    new GaussianBlur(0.4, 3).ApplyInPlace(croppedAreaPart.Bitmap);

                    //Resize to multiples of four
                    using (var bitmapFixedToMultiplyOfFour =
                            new ResizeBilinear(inputSizeAsMultipliesOfFour.Width, inputSizeAsMultipliesOfFour.Height)
                                .Apply(croppedAreaPart.Bitmap))
                    //Resize to 28*XX
                    using (var bitmapDownscaledToExpectedHeight =
                        new ResizeBilinear(inputSizeAfterDownscale.Width, inputSizeAfterDownscale.Height).Apply(
                            bitmapFixedToMultiplyOfFour))
                    //using (var bitmapResized =
                    //            new ResizeBilinear(28, 28)
                    //                .Apply(croppedAreaPart.Bitmap))
                    {
                        //Resize canvas and center to 28*28
                        flattenedResults.Add(new OrderedFlattenedBitmap(croppedAreaPart.Order,
                            CreatedSquaredAndCenteredBitmapFrom(bitmapDownscaledToExpectedHeight)));
                    }
                }

                output = new FlattenedCroppedArea(croppedArea.AreaUsedForCropping, flattenedResults, croppedArea.DocumentId);
            }

            //Make bitmap squared instead of rectangular
            return ProcessingResult<FlattenedCroppedArea>.Success(output);
        }

        public Dimension2D GetInputSizeFromBitmap(Bitmap bitmap)
        {
            return new Dimension2D(bitmap.Width, bitmap.Height);
        }

        public Dimension2D GetDimensionsForResizeBeforeCentering(Dimension2D inputSizeAsMultiplesOfFour)
        {
            return new Dimension2D(
                (int)Math.Ceiling(inputSizeAsMultiplesOfFour.Width / ((double)inputSizeAsMultiplesOfFour.Height / ExpectedSize2D.Height)),
                ExpectedSize2D.Height);
        }

        /// <summary>
        /// Resize bitmap canvas, so that original pixels become unchanged and are centered in new image that has even edges;
        /// Assumes working on form data, ie. width smaller than height
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Square bitmap; ie. for 28x30 image you get 30x30 image with original image centered</returns>
        private byte[] CreatedSquaredAndCenteredBitmapFrom(Bitmap input)
        {

            //Hardly efficient and quite ugly but works
            if (input.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new InvalidOperationException(); //TODO: return result instead
            }

            if (input.Width > input.Height)
            {
                throw new InvalidOperationException(); //TODO: return result instead
            }

            BitmapData bitmapData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height),
                ImageLockMode.ReadWrite, input.PixelFormat);

            var squaredBitmapEdge = input.Height;

            try
            {
                var leftSidePixelsSkipped = (input.Height - input.Width) / 2;

                byte[] dataRead = new byte[input.Width * input.Height];
                Marshal.Copy(bitmapData.Scan0, dataRead, 0, input.Width * input.Height);
                byte[] dataToWrite = new byte[squaredBitmapEdge * squaredBitmapEdge];
                byte[] dataToWrite2 = new byte[squaredBitmapEdge * squaredBitmapEdge];


                //TODO: Clean this up so that it also pivots bytes properly in one go, not two....
                for (int i = 0; i < input.Height; ++i)
                {
                    for (int j = 0; j < input.Width; ++j)
                    {
                        dataToWrite[i * squaredBitmapEdge + j + leftSidePixelsSkipped] =
                            dataRead[i * input.Width + j];
                    }
                }

                for (int i = 0; i < input.Height; ++i)
                {
                    for (int j = 0; j < input.Width; ++j)
                    {
                        dataToWrite2[j * squaredBitmapEdge + i] = dataToWrite[i * squaredBitmapEdge + j];
                    }
                }
                //END OF TODO
                return dataToWrite2;
            }
            finally
            {
                input.UnlockBits(bitmapData);
            }
        }
    }
}
