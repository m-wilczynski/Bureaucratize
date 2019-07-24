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

namespace Bureaucratize.ImageProcessing.Contracts.ProcessingMessages.Details
{
    public class AddedBitmapForPageTwice : IResultDetails
    {
        private readonly int _pageNumber;
        public string DetailsMessageKey => nameof(AddedBitmapForPageTwice);

        public AddedBitmapForPageTwice(int pageNumber)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            _pageNumber = pageNumber;
        }

        public string GetDetails()
        {
            return $"Attempt to add image for page (number: {_pageNumber}) twice (try replacing it instead).";
        }
    }
}
