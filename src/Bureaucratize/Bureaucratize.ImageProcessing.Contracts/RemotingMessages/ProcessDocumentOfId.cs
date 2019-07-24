using System;

namespace Bureaucratize.ImageProcessing.Contracts.RemotingMessages
{
    public class ProcessDocumentOfIdRequest
    {
        public ProcessDocumentOfIdRequest(Guid documentId, Guid requesterId)
        {
            Validate(ref documentId, ref requesterId);

            DocumentId = documentId;
            RequesterId = requesterId;
        }

        public Guid DocumentId { get; }
        public Guid RequesterId { get; }

        private static void Validate(ref Guid documentId, ref Guid requesterId)
        {
            if (documentId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(documentId));
            if (requesterId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(requesterId));
        }
    }

    public class ProcessDocumentOfIdResponse
    {
        public ProcessDocumentOfIdResponse(Guid documentId, Guid requesterId, bool wasAccepted)
        {
            Validate(ref documentId, ref requesterId);

            DocumentId = documentId;
            RequesterId = requesterId;
            WasAccepted = wasAccepted;
        }

        public Guid DocumentId { get; }
        public Guid RequesterId { get; }
        public bool WasAccepted { get; }

        private static void Validate(ref Guid documentId, ref Guid requesterId)
        {
            if (documentId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(documentId));
            if (requesterId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(requesterId));
        }
    }

    public static class ProcessDocumentOfIdExtensions
    {
        public static ProcessDocumentOfIdResponse Accepted(this ProcessDocumentOfIdRequest request)
        {
            return new ProcessDocumentOfIdResponse(request.DocumentId, request.RequesterId, true);
        }

        public static ProcessDocumentOfIdResponse Rejected(this ProcessDocumentOfIdRequest request)
        {
            return new ProcessDocumentOfIdResponse(request.DocumentId, request.RequesterId, false);
        }
    }
}
