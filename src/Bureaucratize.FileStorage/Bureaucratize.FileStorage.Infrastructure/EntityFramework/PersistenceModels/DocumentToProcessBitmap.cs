using System;
using Bureaucratize.FileStorage.Infrastructure.EntityFramework.PersistenceModels.Base;

namespace Bureaucratize.FileStorage.Infrastructure.EntityFramework.PersistenceModels
{
    internal class DocumentToProcessBitmap : PersistenceModel
    {
        public Guid DocumentToProcessId { get; set; }
        public string BitmapPath { get; set; }
        public int Order { get; set; }
        public string FileLabel { get; set; }
    }
}