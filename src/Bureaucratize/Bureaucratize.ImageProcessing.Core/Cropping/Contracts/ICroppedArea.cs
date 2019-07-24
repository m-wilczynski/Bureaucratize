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
using Bureaucratize.ImageProcessing.Contracts.Bitmaps;
using Bureaucratize.ImageProcessing.Core.Common;
using Bureaucratize.Templating.Core.InterestPoints.Contracts;

namespace Bureaucratize.ImageProcessing.Core.Cropping.Contracts
{
    /// <summary>
    /// Result of cropping by <see cref="ITemplateAreasCropper"/>
    /// </summary>
    public interface ICroppedArea : IDisposable
    {
        Guid DocumentId { get; }
        ITemplatePageArea AreaUsedForCropping { get; }
        IReadOnlyCollection<OrderedBitmap> CroppedParts { get; }
    }
}
