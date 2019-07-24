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
using System.Drawing;
using Bureaucratize.Common.Core.Utils;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Models;

namespace Bureaucratize.ImageProcessing.Contracts.Bitmaps
{
    public class OrderedBitmap
    {
        public uint Order { get; }
        public Bitmap Bitmap { get; }
        public string Label { get; set; }

        public OrderedBitmap(uint order, Bitmap bitmap)
        {
            Order = order;
            //Null is acceptable - we might yield no ROI at all, so why bother processing it further
            Bitmap = bitmap;
        }
    }

    public static class OrderedBitmapExtensions
    {
        public static OrderedBitmap AsOrderedBitmap(this OrderedBitmapResource resource)
        {
            return new OrderedBitmap((uint)resource.Order, resource.FileData.AsBitmap());
        }

        public static OrderedBitmap AsOrderedBitmap(this OrderedBitmapToSave bitmapToSave)
        {
            return new OrderedBitmap((uint)bitmapToSave.Order, bitmapToSave.FileData.AsBitmap());
        }
    }
}
