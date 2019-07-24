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
using Bureaucratize.Common.Core;

namespace Bureaucratize.ImageProcessing.Contracts.Recognition
{
    public class RecognizedChoicePart : Identifiable, IRecognizedPart<bool>
    {
        private RecognizedChoicePart() { }

        public RecognizedChoicePart(Guid areaUsedForRecognitionId, Guid documentId, bool recognitionOutput, Guid? id = null)
            : base (id)
        {
            if (areaUsedForRecognitionId == Guid.Empty) throw new ArgumentNullException(nameof(areaUsedForRecognitionId));
            if (documentId == Guid.Empty) throw new ArgumentNullException(nameof(documentId));

            AreaUsedForRecognitionId = areaUsedForRecognitionId;
            DocumentId = documentId;
            RecognitionOutput = recognitionOutput;
        }

        public Guid AreaUsedForRecognitionId { get; private set; }
        public Guid DocumentId { get; private set; }
        public bool RecognitionOutput { get; private set; }
        public float RecognitionCertaintyPercent => 1;
        public string ResultStringified => RecognitionOutput ? "Wybrano" : "Nie wybrano";
        public string AreaName { get; set; }
    }
}
