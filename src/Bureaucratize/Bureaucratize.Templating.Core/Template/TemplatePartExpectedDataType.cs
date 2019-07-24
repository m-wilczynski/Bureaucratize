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
using Bureaucratize.FileStorage.Contracts;

namespace Bureaucratize.Templating.Core.Template
{
    /// <summary>
    /// Defines possible data to be expected from filled template part
    /// </summary>
    [Flags]
    public enum TemplatePartExpectedDataType
    {
        Digits = 1,
        Letters = 2,
        SpecialChars = 4,
        Choice = 8,
        Alphanumeric = Digits | Letters,
        AnyText = Digits | Letters | SpecialChars
    }

    public static class ExpectedDataTypeExtensions
    {
        public static ImageRecognitionExpectedData AsFileStorageModel(
            this TemplatePartExpectedDataType expectedData)
        {
            switch (expectedData)
            {
                case TemplatePartExpectedDataType.Digits:
                    return ImageRecognitionExpectedData.DigitsModel;
                case TemplatePartExpectedDataType.Letters:
                    return ImageRecognitionExpectedData.LettersModel;
                case TemplatePartExpectedDataType.SpecialChars:
                case TemplatePartExpectedDataType.Choice:
                case TemplatePartExpectedDataType.Alphanumeric:
                case TemplatePartExpectedDataType.AnyText:
                    return ImageRecognitionExpectedData.AlphanumericsModel;
                default:
                    throw new NotImplementedException();
            }
        }

        public static string AsUserReadableString(this TemplatePartExpectedDataType expectedData)
        {
            switch (expectedData)
            {
                case TemplatePartExpectedDataType.Digits:
                    return "Cyfry";
                case TemplatePartExpectedDataType.Letters:
                    return "Litery";
                case TemplatePartExpectedDataType.SpecialChars:
                    return "Znaki specjalne";
                case TemplatePartExpectedDataType.Choice:
                    return "Wybór";
                case TemplatePartExpectedDataType.Alphanumeric:
                    return "Cyfry i litery";
                case TemplatePartExpectedDataType.AnyText:
                    return "Dowolny tekst";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
