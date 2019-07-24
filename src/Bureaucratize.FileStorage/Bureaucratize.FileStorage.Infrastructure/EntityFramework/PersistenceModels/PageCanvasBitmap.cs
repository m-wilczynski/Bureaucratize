using System;
using Bureaucratize.FileStorage.Infrastructure.EntityFramework.PersistenceModels.Base;

namespace Bureaucratize.FileStorage.Infrastructure.EntityFramework.PersistenceModels
{
    internal class PageCanvasBitmap : PersistenceModel
    {
        public Guid TemplatePageId { get; set; }
        public Guid TemplateId { get; set; }
        public string BitmapPath { get; set; }
        public string FileLabel { get; set; }
    }
}
