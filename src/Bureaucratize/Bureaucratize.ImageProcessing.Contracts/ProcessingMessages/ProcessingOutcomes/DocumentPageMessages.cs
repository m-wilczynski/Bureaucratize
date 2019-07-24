using System;
using System.Collections.Generic;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes.Models;
using Bureaucratize.ImageProcessing.Contracts.Recognition;

namespace Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes
{
    public abstract class DocumentPageMessage
    {
        public string Scope => "Page";

        protected DocumentPageMessage(Guid documentId, int pageNumber)
        {
            DocumentId = documentId;
            PageNumber = pageNumber;
        }

        public Guid DocumentId { get; }
        public int PageNumber { get; }
    }

    public class DocumentPageProcessingStarted : DocumentPageMessage
    {
        public DocumentPageProcessingStarted(Guid documentId, int pageNumber) : base(documentId, pageNumber)
        {
        }
    }

    public class DocumentPageProcessingFailed : DocumentPageMessage
    {
        public DocumentPageProcessingFailed(Guid documentId, int pageNumber) : base(documentId, pageNumber)
        {
        }
    }

    public class DocumentPageProcessingCompleted : DocumentPageMessage
    {
        public ProcessedDocumentPage ProcessedPage { get; }
        public List<RecognizedTextPart> ProcessedTextAreas { get; set; }
        public List<RecognizedChoicePart> ProcessedChoiceAreas { get; set; }

        public DocumentPageProcessingCompleted(Guid documentId, ProcessedDocumentPage processedPage) 
            : base(documentId, processedPage.PageNumber)
        {
            ProcessedPage = processedPage;
            ProcessedChoiceAreas = processedPage.RecognizedChoiceParts;
            ProcessedTextAreas = processedPage.RecognizedTextParts;
        }
    }
}
