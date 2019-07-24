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

using System.Linq;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.ImageProcessing.Contracts.Recognition;
using Bureaucratize.ImageProcessing.Core.Cropping.Contracts;
using Bureaucratize.ImageProcessing.Core.Recognition.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Recognition
{
    public class FindAnyInputHandwrittenChoiceRecognizer : IHandwrittenChoiceRecognizer
    {
        public ProcessingResult<IRecognizedPart<bool>> RecognizeFrom(ICroppedArea croppedArea)
        {
            if (croppedArea.CroppedParts.Count != 1)
            {
                //TODO: ResultDetails for - expected only one element
                return ProcessingResult<IRecognizedPart<bool>>.Failure(null);
            }

            var croppedElement = croppedArea.CroppedParts.Single();
            if (croppedElement == null)
            {
                //TODO: ResultDetails for - empty element in CroppedArea
                return ProcessingResult<IRecognizedPart<bool>>.Failure(null);
            }

            return ProcessingResult<IRecognizedPart<bool>>.Success(
                new RecognizedChoicePart(croppedArea.AreaUsedForCropping.Id, 
                    croppedArea.DocumentId, 
                    //Empty field - no input; otherwise - is checked via handwriting
                    croppedElement.Bitmap != null));
        }
    }
}
