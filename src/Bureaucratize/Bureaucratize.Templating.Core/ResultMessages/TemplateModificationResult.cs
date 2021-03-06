﻿/*
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

namespace Bureaucratize.Templating.Core.ResultMessages
{
    public class TemplateModificationResult
    {
        public bool Successful { get; }
        public IResultDetails Details { get; }

        public TemplateModificationResult(bool successful, IResultDetails details)
        {
            Successful = successful;
            Details = details;
        }

        public static TemplateModificationResult Success(IResultDetails details = null) => new TemplateModificationResult(true, details);
        public static TemplateModificationResult Failure(IResultDetails details) => new TemplateModificationResult(false, details);
    }

    public class TemplateModificationResult<TResult>
    {
        public bool Successful { get; }
        public TResult Result { get; }
        public IResultDetails Details { get; }

        public TemplateModificationResult(bool successful, TResult result, IResultDetails details)
        {
            Successful = successful;
            Result = result;
            Details = details;
        }

        public static TemplateModificationResult<TResult> Success(TResult result, IResultDetails details = null)
            => new TemplateModificationResult<TResult>(true, result, details);
        public static TemplateModificationResult<TResult> Failure(IResultDetails details)
            => new TemplateModificationResult<TResult>(false, default(TResult), details);
    }
}
