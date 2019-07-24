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
    public class ProcessingResult<TResult>
    {
        public ProcessingResult(StepOutcome outcome, TResult result, IResultDetails details)
        {
            Result = result;
            Outcome = outcome;
            Details = details;
        }

        public StepOutcome Outcome { get; }
        public TResult Result { get; }
        public IResultDetails Details { get; }

        public static ProcessingResult<TResult> Success(TResult result, IResultDetails details = null) => new ProcessingResult<TResult>(StepOutcome.Success, result, details);
        public static ProcessingResult<TResult> Failure(IResultDetails details) => new ProcessingResult<TResult>(StepOutcome.Failure, default(TResult), details);
        public static ProcessingResult<TResult> Skipped(IResultDetails details) => new ProcessingResult<TResult>(StepOutcome.Skipped, default(TResult), details);
    }

    public enum StepOutcome
    {
        Skipped,
        Success,
        Failure
    }
}
