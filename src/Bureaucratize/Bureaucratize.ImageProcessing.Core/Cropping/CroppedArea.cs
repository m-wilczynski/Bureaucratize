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
using Bureaucratize.ImageProcessing.Contracts.Bitmaps;
using Bureaucratize.ImageProcessing.Core.Common;
using Bureaucratize.ImageProcessing.Core.Cropping.Contracts;
using Bureaucratize.Templating.Core.InterestPoints.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Cropping
{
    public class CroppedArea : ICroppedArea
    {
        public CroppedArea(ITemplatePageArea areaUsed, IReadOnlyCollection<OrderedBitmap> croppedParts, Guid documentId)
        {
            if (areaUsed == null)
                throw new ArgumentNullException(nameof(areaUsed));
            if (croppedParts == null)
                throw new ArgumentNullException(nameof(croppedParts));
            if (documentId == Guid.Empty)
                throw new ArgumentNullException(nameof(documentId));

            AreaUsedForCropping = areaUsed;
            CroppedParts = croppedParts;
            DocumentId = documentId;
        }

        public Guid DocumentId { get; }
        public ITemplatePageArea AreaUsedForCropping { get; }
        public IReadOnlyCollection<OrderedBitmap> CroppedParts { get; }

        public void Dispose()
        {
            if (CroppedParts == null) return;
            foreach (var part in CroppedParts)
            {
                part?.Bitmap?.Dispose();
            }
        }
    }
}
