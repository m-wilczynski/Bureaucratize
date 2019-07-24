using System;

namespace Bureaucratize.FileStorage.Contracts.Models
{
    public class TemplatePageCanvasBitmapResource
    {
        public Guid TemplatePageId { get; set; }
        public byte[] FileData { get; set; }
        public BitmapFiletype Filetype { get; set; }
    }
}
