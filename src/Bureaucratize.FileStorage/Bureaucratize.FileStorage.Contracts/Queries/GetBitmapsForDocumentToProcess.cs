using System;
using Bureaucratize.FileStorage.Contracts.Queries.Base;

namespace Bureaucratize.FileStorage.Contracts.Queries
{
    public class GetBitmapsForDocumentToProcess : IFileStorageQuery
    {
        public Guid DocumentId { get; set; }
    }
}
