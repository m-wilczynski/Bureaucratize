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
using System.Linq;
using Bureaucratize.Common.Core.Infrastructure.FileStore;
using Bureaucratize.FileStorage.Contracts.Queries;
using Bureaucratize.ImageProcessing.Contracts.ProcessingMessages;
using Bureaucratize.ImageProcessing.Contracts.Recognition;
using Bureaucratize.ImageProcessing.Core.Cleaning;
using Bureaucratize.ImageProcessing.Core.Recognition.Contracts;
using Bureaucratize.Templating.Core.Template;
using CNTK;

namespace Bureaucratize.ImageProcessing.Core.Recognition
{
    public class DeepLearningHandwrittenInputRecognizer : IHandwrittenInputRecognizer
    {
        private readonly DeepLearningLabelMapConfiguration _labelMapConfiguration;
        private readonly IResourceQueryHandler<GetImageRecognitionModel, byte[]> _getRecognitionModelHandler;
        private readonly DeviceDescriptor _device = DeviceDescriptor.CPUDevice;

        public DeepLearningHandwrittenInputRecognizer(
            DeepLearningLabelMapConfiguration labelMapConfiguration,
            IResourceQueryHandler<GetImageRecognitionModel, byte[]> getRecognitionModelHandler)
        {
            if (getRecognitionModelHandler == null) throw new ArgumentNullException(nameof(getRecognitionModelHandler));
            _labelMapConfiguration = labelMapConfiguration ?? throw new ArgumentNullException(nameof(labelMapConfiguration));

            _getRecognitionModelHandler = getRecognitionModelHandler;
        }

        public ProcessingResult<IRecognizedPart<string>> RecognizeFrom(FlattenedCroppedArea croppedArea)
        {
            var expectedData = croppedArea.AreaUsedForCropping.ExpectedData.AsFileStorageModel();
            var modelBytes = _getRecognitionModelHandler.Handle(
                new GetImageRecognitionModel
                {
                    ExpectedData = expectedData
                });

            var model = Function.Load(modelBytes, _device);

            try
            {
                var areaPartsPredictions = new List<OrderedRecognitionOutput>();

                foreach (var flattenedBitmap in croppedArea.FlattenedBitmaps.OrderBy(fb => fb.Order))
                {
                    if (flattenedBitmap.FlattenedBitmap == null)
                    {
                        areaPartsPredictions.Add(new OrderedRecognitionOutput(default(char), 1f, flattenedBitmap.Order));
                        continue;
                    }

                    Variable inputVar = model.Arguments[0];
                    NDShape inputShape = inputVar.Shape;

                    List<float> flattenedBitmapFeatures =
                        flattenedBitmap.FlattenedBitmap.Select(feat => (float)feat).ToList();

                    // Create input data map
                    var inputDataMap = new Dictionary<Variable, Value>();
                    var inputVal = Value.CreateBatch(inputShape, flattenedBitmapFeatures, _device);
                    inputDataMap.Add(inputVar, inputVal);

                    Variable outputVar = model.Output;

                    // Create output data map. Using null as Value to indicate using system allocated memory.
                    // Alternatively, create a Value object and add it to the data map.
                    var outputDataMap = new Dictionary<Variable, Value>();
                    outputDataMap.Add(outputVar, null);

                    // Start evaluation on the device
                    model.Evaluate(inputDataMap, outputDataMap, _device);

                    // Get evaluate result as dense output
                    var outputVal = outputDataMap[outputVar];
                    var outputData = outputVal.GetDenseData<float>(outputVar);
                    Dictionary<int, float> outputPred = new Dictionary<int, float>();

                    for (int i = 0; i < outputData[0].Count; i++)
                    {
                        outputPred.Add(i, outputData[0][i]);
                    }

                    var topPrediction = outputPred.GetTopRelativePrediction();
                    areaPartsPredictions.Add(new OrderedRecognitionOutput(
                        _labelMapConfiguration.GetCharFromLabel(croppedArea.AreaUsedForCropping.ExpectedData, topPrediction.EvaluationLabel),
                        topPrediction.RelativePercentageScore,
                        flattenedBitmap.Order));
                }

                return ProcessingResult<IRecognizedPart<string>>.Success(
                    new RecognizedTextPart(croppedArea.AreaUsedForCropping.Id, areaPartsPredictions, croppedArea.DocumentId));
            }
            catch (Exception ex)
            {
                //TODO: Return failure
                throw ex;
            }
        }
    }

    public static class PredictionExtensions
    {
        public static Prediction GetTopRelativePrediction(this Dictionary<int, float> predictions)
        {
            var topFive = predictions.OrderByDescending(kvp => kvp.Value).Take(5).ToDictionary(el => el.Key, el => el.Value);
            var topRank = topFive.First();

            return new Prediction
            {
                EvaluationLabel = (char)topRank.Key,
                RelativePercentageScore = topRank.Value / topFive.Values.Sum()
            };
        }
    }

    public struct Prediction
    {
        public float RelativePercentageScore;
        public int EvaluationLabel;
    }
}
