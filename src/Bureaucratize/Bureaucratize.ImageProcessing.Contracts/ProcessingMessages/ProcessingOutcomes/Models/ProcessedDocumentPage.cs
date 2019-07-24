/*
   Copyright (c) 2018 Michał Wilczyński

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using Bureaucratize.Common.Core;
using Bureaucratize.ImageProcessing.Contracts.Recognition;

namespace Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.ProcessingOutcomes.Models
{
    public class ProcessedDocumentPage : Identifiable
    {
        private ProcessedDocumentPage() { }

        public ProcessedDocumentPage(List<RecognizedTextPart> recognizedTextParts,
            List<RecognizedChoicePart> recognizedChoiceParts, int pageNumber)
        {
            if (recognizedTextParts == null) throw new ArgumentNullException(nameof(recognizedTextParts));
            if (recognizedChoiceParts == null) throw new ArgumentNullException(nameof(recognizedChoiceParts));
            if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));

            RecognizedTextParts = recognizedTextParts;
            RecognizedChoiceParts = recognizedChoiceParts;
            PageNumber = pageNumber;
        }

        public int PageNumber { get; private set; }
        public List<RecognizedTextPart> RecognizedTextParts { get; private set; }
        public List<RecognizedChoicePart> RecognizedChoiceParts { get; private set; }
    }
}
