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

namespace Bureaucratize.FileStorage.Contracts
{
    public enum BitmapFiletype
    {
        Jpg = 1,
        Png = 2,
        Bmp = 3,
        Tiff = 4
    }

    public static class BitmapFiletypeExtensions
    {
        public static string AsFileExtension(this BitmapFiletype fileTypeEnum)
        {
            return "." + fileTypeEnum.ToString().ToLower();
        }

        public static BitmapFiletype AsBitmapFiletype(this string fileName)
        {
            var fileNameParts = fileName.Split('.');

            if (fileNameParts.Length < 2)
                throw new InvalidOperationException("Not a filename!");

            switch (fileNameParts[fileNameParts.Length - 1].ToLower())
            {
                case "jpg":
                case "jpeg":
                    return BitmapFiletype.Jpg;
                case "png":
                    return BitmapFiletype.Png;
                case "bmp":
                    return BitmapFiletype.Bmp;
                case "tiff":
                    return BitmapFiletype.Tiff;
                default:
                    throw new NotImplementedException(fileNameParts[fileNameParts.Length - 1].ToLower() 
                                                      + " has no coresponding enum value");

            }
        }
    }
}
