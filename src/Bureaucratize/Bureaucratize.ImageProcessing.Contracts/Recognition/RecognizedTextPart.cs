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
using System.Linq;
using Bureaucratize.Common.Core;

namespace Bureaucratize.ImageProcessing.Contracts.Recognition
{
    public class RecognizedTextPart : Identifiable, IRecognizedPart<string>
    {
        private RecognizedTextPart() { }

        public RecognizedTextPart(Guid pageAreaUsedId, ICollection<OrderedRecognitionOutput> recognitionOutputs,
            Guid documentId, Guid? id = null) : base(id)
        {
            if (pageAreaUsedId == Guid.Empty)
                throw new ArgumentNullException(nameof(pageAreaUsedId));

            if (documentId == Guid.Empty)
                throw new ArgumentNullException(nameof(documentId));

            if (recognitionOutputs == null || recognitionOutputs.Count == 0)
                throw new ArgumentNullException(nameof(recognitionOutputs));

            DocumentId = documentId;
            AreaUsedForRecognitionId = pageAreaUsedId;

            RecognitionOutput = new string(recognitionOutputs.Where(ro => ro.Label != default(char)).Select(ro => ro.Label).ToArray());
            RecognitionCertaintyPercent = recognitionOutputs.Select(ro => ro.CertaintyPercentage).Average();
        }

        public Guid AreaUsedForRecognitionId { get; private set; }
        public Guid DocumentId { get; private set; }
        public string RecognitionOutput { get; private set; }
        public float RecognitionCertaintyPercent { get; private set; }
        public string ResultStringified => string.IsNullOrWhiteSpace(RecognitionOutput) ? "{Pusty}" : RecognitionOutput;
        public string AreaName { get; set; }
    }
}
