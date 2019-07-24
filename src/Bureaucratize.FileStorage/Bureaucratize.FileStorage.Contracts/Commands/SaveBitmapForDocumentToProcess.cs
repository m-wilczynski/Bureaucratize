using System;
using Bureaucratize.FileStorage.Contracts.Commands.Base;

namespace Bureaucratize.FileStorage.Contracts.Commands
{
    public class SavePageBitmapForDocumentToProcess : SaveBitmapCommand
    {
        public Guid DocumentId { get; set; }
        public int PageNumber { get; set; }
    }
}
