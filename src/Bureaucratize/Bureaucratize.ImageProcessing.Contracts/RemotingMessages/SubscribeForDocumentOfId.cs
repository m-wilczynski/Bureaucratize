using System;

namespace Bureaucratize.ImageProcessing.Contracts.RemotingMessages
{
    public class SubscribeForDocumentOfId
    {
        public SubscribeForDocumentOfId(Guid documentId)
        {
            DocumentId = documentId;
        }

        public Guid DocumentId { get; private set; }
    }
}
