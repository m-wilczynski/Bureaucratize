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

using Bureaucratize.Common.Core;

namespace Bureaucratize.ImageProcessing.Contracts.ProcessingMessages
{
    public class DocumentModificationResult
    {
        public bool Successful { get; }
        public IResultDetails Details { get; }

        public DocumentModificationResult(bool successful, IResultDetails details)
        {
            Successful = successful;
            Details = details;
        }

        public static DocumentModificationResult Success(IResultDetails details = null) => new DocumentModificationResult(true, details);
        public static DocumentModificationResult Failure(IResultDetails details) => new DocumentModificationResult(false, details);
    }

    public class DocumentModificationResult<TResult>
    {
        public bool Successful { get; }
        public TResult Result { get; }
        public IResultDetails Details { get; }

        public DocumentModificationResult(bool successful, TResult result, IResultDetails details)
        {
            Successful = successful;
            Result = result;
            Details = details;
        }

        public static DocumentModificationResult<TResult> Success(TResult result, IResultDetails details = null)
            => new DocumentModificationResult<TResult>(true, result, details);
        public static DocumentModificationResult<TResult> Failure(IResultDetails details)
            => new DocumentModificationResult<TResult>(false, default(TResult), details);
    }
}
