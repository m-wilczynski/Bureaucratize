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
using System.Threading.Tasks;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.FileStorage.Contracts;
using Bureaucratize.FileStorage.Contracts.Queries;
using Bureaucratize.Templating.Core.Template;

namespace Bureaucratize.ImageProcessing.Core.Recognition
{
    public class DeepLearningLabelMapConfiguration
    {
        private readonly IResourceQueryHandler<GetImageRecognitionLabelMap, string> _getLabelMapsHandler;
        public readonly string DigitsMapUrl;
        public readonly string LettersMapUrl;
        public readonly string AlphanumericMapUrl;

        private Dictionary<int, char> _digitLabelToCharMap = null;
        private Dictionary<int, char> _letterLabelToCharMap = null;
        private Dictionary<int, char> _alphanumericLabelToCharMap = null;

        public DeepLearningLabelMapConfiguration(IResourceQueryHandler<GetImageRecognitionLabelMap, string> getLabelMapsHandler)
        {
            if (getLabelMapsHandler == null) throw new ArgumentNullException(nameof(getLabelMapsHandler));
            _getLabelMapsHandler = getLabelMapsHandler;
        }

        //TODO: Better error handling
        public bool LoadDataTypeMaps()
        {
            if (_digitLabelToCharMap == null)
            {
                _digitLabelToCharMap = FetchMapFromUrl(ImageRecognitionExpectedData.DigitsModel);
                if (_digitLabelToCharMap == null) return false;
            }

            if (_letterLabelToCharMap == null)
            {
                _letterLabelToCharMap = FetchMapFromUrl(ImageRecognitionExpectedData.LettersModel);
                if (_digitLabelToCharMap == null) return false;
            }

            //if (_alphanumericLabelToCharMap == null)
            //{
            //    _alphanumericLabelToCharMap = await FetchMapFromUrl(AlphanumericMapUrl);
            //    if (_digitLabelToCharMap == null) return false;
            //}

            return true;
        }

        //TODO: Better error handling
        public char GetCharFromLabel(TemplatePartExpectedDataType dataTypeToEvaluate, int label)
        {
            var loadResult = LoadDataTypeMaps();

            if (!loadResult)
                throw new InvalidOperationException();

            switch (dataTypeToEvaluate)
            {
                case TemplatePartExpectedDataType.Digits:
                    return _digitLabelToCharMap[label];
                case TemplatePartExpectedDataType.Letters:
                    return _letterLabelToCharMap[label];
                case TemplatePartExpectedDataType.Alphanumeric:
                case TemplatePartExpectedDataType.AnyText:
                case TemplatePartExpectedDataType.SpecialChars:
                case TemplatePartExpectedDataType.Choice:
                    return _alphanumericLabelToCharMap[label];
                default:
                    //TODO: Throw instead?
                    return _alphanumericLabelToCharMap[label];
            }
        }

        //TODO: Better error handling
        public Dictionary<int, char> FetchMapFromUrl(ImageRecognitionExpectedData expectedData)
        {
            try
            {
                var labelMap = new Dictionary<int, char>();
                var content = _getLabelMapsHandler.Handle(new GetImageRecognitionLabelMap { ExpectedData = expectedData });

                foreach (var line in content.Split(new[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = line.Split(' ');
                    labelMap.Add(int.Parse(parts[0]), (char)int.Parse(parts[1]));
                }

                return labelMap;
            }
            catch
            {
                return null;
            }
        }
    }
}
