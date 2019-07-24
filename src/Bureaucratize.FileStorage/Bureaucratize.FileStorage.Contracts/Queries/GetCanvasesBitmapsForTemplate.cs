using System;
using Bureaucratize.FileStorage.Contracts.Queries.Base;

namespace Bureaucratize.FileStorage.Contracts.Queries
{
    public class GetCanvasesBitmapsForTemplate : IFileStorageQuery
    {
        public Guid TemplateId { get; set; }
    }
}
