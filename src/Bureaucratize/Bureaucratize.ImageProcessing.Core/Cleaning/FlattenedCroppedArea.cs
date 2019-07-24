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
using Bureaucratize.ImageProcessing.Core.Common;
using Bureaucratize.Templating.Core.InterestPoints.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Cleaning
{
    public class FlattenedCroppedArea
    {
        public FlattenedCroppedArea(ITemplatePageArea areaUsedForCropping, 
            ICollection<OrderedFlattenedBitmap> croppedBitmaps, Guid documentId)
        {
            if (croppedBitmaps == null || croppedBitmaps.Count == 0)
                throw new ArgumentNullException(nameof(croppedBitmaps));
            if (documentId == Guid.Empty)
                throw new ArgumentNullException(nameof(documentId));

            AreaUsedForCropping = areaUsedForCropping ?? throw new ArgumentNullException(nameof(areaUsedForCropping));
            FlattenedBitmaps = croppedBitmaps;
            DocumentId = documentId;
        }
        
        public Guid DocumentId { get; }
        public ITemplatePageArea AreaUsedForCropping { get; }
        public ICollection<OrderedFlattenedBitmap> FlattenedBitmaps { get; }
    }
}
