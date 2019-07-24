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

namespace Bureaucratize.Common.Core.CommonDetails
{
    //TODO: Should I consist actual exception details?
    public class UncaughtException : IResultDetails
    {
        public string DetailsMessageKey => nameof(UncaughtException);

        public string StackTrace { get; }
        public string Message { get; }
        public string InnerExceptionMessage { get; }

        public UncaughtException(Exception ex)
        {
            StackTrace = ex.StackTrace;
            Message = ex.Message;

            if (ex.InnerException != null)
            {
                InnerExceptionMessage = ex.InnerException.Message;
            }
        }

        public string GetDetails()
        {
            return "Unexpected error occured.";
        }
    }
}
