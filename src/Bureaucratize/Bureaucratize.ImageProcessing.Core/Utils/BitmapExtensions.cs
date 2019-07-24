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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Bureaucratize.Common.Core.Structures;

namespace Bureaucratize.ImageProcessing.Core.Utils
{
    public static class BitmapExtensions
    {
        public static FlattenedBitmap AsFlattenedByteArray(this Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite, bitmap.PixelFormat);

            try
            {
                byte[] dataRead = new byte[bitmapData.Stride * bitmap.Height];
                byte[,] dataRead2D = new byte[bitmap.Height, bitmapData.Stride];
                Marshal.Copy(bitmapData.Scan0, dataRead, 0, bitmap.Width * bitmap.Height);

                for (int i = 0; i < bitmap.Height; ++i)
                {
                    for (int j = 0; j < bitmapData.Stride; ++j)
                    {
                        dataRead2D[i, j] = dataRead[i * (bitmapData.Stride) + j];
                    }
                }

                return new FlattenedBitmap(bitmapData.Width, bitmapData.Height, bitmapData.Stride, dataRead2D);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }

    public class FlattenedBitmap
    {
        public readonly Dimension2D Dimension;
        public readonly int Stride;
        public readonly byte[,] Data;

        public FlattenedBitmap(int width, int height, int stride, byte[,] data)
        {
            Dimension = new Dimension2D(width, height);
            Stride = stride;
            Data = data;
        }

        public int Width => Dimension.Width;
        public int Height => Dimension.Height;
    }
}
