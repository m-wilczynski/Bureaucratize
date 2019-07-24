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
using System.IO;
using System.Security;

namespace Bureaucratize.Common.Core.CommonDetails
{
    public class FilesystemFetchFailure : IResultDetails
    {
        public string DetailsMessageKey => nameof(FilesystemFetchFailure);
        private readonly FilesystemFetchFailureType _failureType;

        public FilesystemFetchFailure(Exception ex)
        {
            _failureType = FromException(ex);
        }

        public FilesystemFetchFailure(FilesystemFetchFailureType failureType)
        {
            _failureType = failureType;
        }

        public string GetDetails()
        {
            return $"Could not retrieve resource from filesystem. Reason: {_failureType.ToString()}";
        }

        private static FilesystemFetchFailureType FromException(Exception ex)
        {
            switch (ex)
            {
                case null:
                    return FilesystemFetchFailureType.Other;
                case PathTooLongException ptl:
                    return FilesystemFetchFailureType.PathTooLong;
                case IOException io:
                    return FilesystemFetchFailureType.FileNotFound;
                case SecurityException sec:
                case UnauthorizedAccessException uacc:
                    return FilesystemFetchFailureType.AccessDenied;
                default:
                    return FilesystemFetchFailureType.Other;
            }
        }
    }

    public enum FilesystemFetchFailureType
    {
        Other,
        FileNotFound,
        PathTooLong,
        AccessDenied,
        CreatingBitmapFailed
    }
}
