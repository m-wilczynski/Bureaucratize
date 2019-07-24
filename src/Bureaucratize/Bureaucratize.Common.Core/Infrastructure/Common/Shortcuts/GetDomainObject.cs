using System;

namespace Bureaucratize.Common.Core.Infrastructure.Common.Shortcuts
{
    public class GetDomainObject<TObject> : IQuery where TObject : Identifiable
    {
        public Guid ObjectId { get; set; }
    }
}
