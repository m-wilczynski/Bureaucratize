using System;

namespace Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes
{
    public abstract class DocumentMessage
    {
        public string Scope => "Document";

        protected DocumentMessage(Guid documentId)
        {
            DocumentId = documentId;
        }

        public Guid DocumentId { get; }    
    }

    public class DocumentProcessingStarted : DocumentMessage
    {
        public DocumentProcessingStarted(Guid documentId) : base(documentId)
        {
        }
    }

    public class DocumentProcessingFailed : DocumentMessage
    {
        public DocumentProcessingFailed(Guid documentId) : base(documentId)
        {
        }
    }

    public class DocumentProcessingCompleted : DocumentMessage
    {
        public DocumentProcessingCompleted(Guid documentId) : base(documentId)
        {
        }
    }

}
