using System;
using System.Collections.Generic;
using Bureaucratize.Common.Core.Infrastructure.Common;
using Bureaucratize.ImageProcessing.Contracts.Recognition;

namespace Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes
{
    public abstract class DocumentPageAreaMessage
    {
        public string Scope => "PageArea";

        protected DocumentPageAreaMessage(Guid documentId, int pageNumber, string areaName)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (string.IsNullOrWhiteSpace(areaName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(areaName));

            DocumentId = documentId;
            PageNumber = pageNumber;
            AreaName = areaName;
        }

        public Guid DocumentId { get; }
        public int PageNumber { get; }
        public string AreaName { get; }
    }

    public class DocumentPageAreaProcessingStarted : DocumentPageAreaMessage
    {
        public DocumentPageAreaProcessingStarted(Guid documentId, int pageNumber, string areaName)
            : base(documentId, pageNumber, areaName)
        {
        }
    }

    public class DocumentPageAreaProcessingFailed : DocumentPageAreaMessage
    {
        public DocumentPageAreaProcessingFailed(Guid documentId, int pageNumber, string areaName)
            : base(documentId, pageNumber, areaName)
        {
        }
    }

    public class DocumentPageTextAreaProcessingCompleted : DocumentPageAreaMessage, ICommand
    {
        public IRecognizedPart<string> RecognitionResult { get; }
        public string AreaTypeText => "Pole tekstowe";
        public string ResultStringified => RecognitionResult.ResultStringified;

        public DocumentPageTextAreaProcessingCompleted(Guid documentId, int pageNumber, string areaName, 
            IRecognizedPart<string> recognitionResult) 
            : base(documentId, pageNumber, areaName)
        {
            RecognitionResult = recognitionResult;
        }
    }

    public class DocumentPageChoiceAreaProcessingCompleted : DocumentPageAreaMessage
    {
        public IRecognizedPart<bool> RecognitionResult { get; }
        public string AreaTypeText => "Pole wyboru";
        public string ResultStringified => RecognitionResult.ResultStringified;

        public DocumentPageChoiceAreaProcessingCompleted(Guid documentId, int pageNumber, string areaName,
            IRecognizedPart<bool> recognitionResult)
            : base(documentId, pageNumber, areaName)
        {
            RecognitionResult = recognitionResult;
        }
    }
}
