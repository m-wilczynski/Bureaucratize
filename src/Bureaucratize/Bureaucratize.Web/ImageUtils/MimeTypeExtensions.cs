using System;
using Bureaucratize.FileStorage.Contracts;

namespace Bureaucratize.Web.ImageUtils
{
    public static class MimeTypeExtensions
    {
        public static string AsMimeType(this BitmapFiletype filetype)
        {
            switch (filetype)
            {
                case BitmapFiletype.Jpg:
                    return "image/jpeg";
                case BitmapFiletype.Png:
                    return "image/png";
                default:
                    throw new NotImplementedException(nameof(filetype));
            }
        }
    }
}
