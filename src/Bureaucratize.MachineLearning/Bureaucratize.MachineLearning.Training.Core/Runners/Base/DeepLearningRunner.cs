using Bureaucratize.MachineLearning.Training.Core.Definitions;
using Bureaucratize.MachineLearning.Training.Core.NeuralNetworks;
using Bureaucratize.MachineLearning.Training.Core.Runners.Output;
using CNTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bureaucratize.MachineLearning.Training.Core.Runners.Base
{
    public abstract class DeepLearningRunner : IDisposable
    {
        public readonly DeviceDescriptor Device;
        public readonly TrainingSessionConfiguration Configuration;
        protected readonly IMessagePrinter MessagePrinter;
        protected PreparedLearningDataset TrainingDataset = null;

        protected DeepLearningRunner(DeviceDescriptor device, TrainingSessionConfiguration configuration, IMessagePrinter printer)
        {
            Device = device ?? throw new System.ArgumentNullException(nameof(device));

            //TODO: User more detailed validation, ie. check all required props/fields for runner to work
            Configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
            this.MessagePrinter = printer ?? throw new ArgumentNullException(nameof(printer));
        }

        public virtual string FeatureStreamName => "features";
        public virtual string LabelsStreamName => "labels";
        public virtual string ClassifierName => "classifierOutput";
        public virtual string LossFunctionName => "lossFunction";
        public virtual string ClassificationErrorName => "classificationError";

        protected IList<StreamConfiguration> GetStreamConfigFrom(ITrainingDatasetDefinition definition)
        {
            return new List<StreamConfiguration>
            {
                new StreamConfiguration(FeatureStreamName, definition.SingleElementSize),
                new StreamConfiguration(LabelsStreamName, definition.LabelsAmount)
            };
        }

        public TrainingSessionResult RunUsing(ITrainingDatasetDefinition datasetDefinition)
        {
            CleanUp();
            BuildNeuralNetwork(datasetDefinition);
            PrepareTrainingData(datasetDefinition);
            ConfigureTrainer();
            TrainNetwork(datasetDefinition);
            SaveResults(datasetDefinition);
            //TODO: Make test dataset volume configurable!
            EvaluateModel(datasetDefinition, Configuration.PersistenceConfig.GetTrainingResultFileNamePathFor(datasetDefinition), 1000);

            return null;
        }

        /// <summary>
        /// Cleanup in case of reused runner instance
        /// </summary>
        protected abstract void CleanUp();

        /// <summary>
        /// Lock and set up neural network
        /// </summary>
        protected abstract void BuildNeuralNetwork(ITrainingDatasetDefinition datasetDefinition);

        /// <summary>
        /// Prepare datasets for usage in DNN
        /// </summary>
        protected abstract void PrepareTrainingData(ITrainingDatasetDefinition datasetDefinition);

        /// <summary>
        /// Configure Trainer and Learner for DNN
        /// </summary>
        protected abstract void ConfigureTrainer();

        /// <summary>
        /// Train network with provided batches
        /// </summary>
        protected abstract void TrainNetwork(ITrainingDatasetDefinition datasetDefinition);

        /// <summary>
        /// Save training results
        /// </summary>
        protected abstract void SaveResults(ITrainingDatasetDefinition datasetDefinition);

        /// <summary>
        /// Evaluate how good (or bad) training went on test data
        /// </summary>
        protected virtual void EvaluateModel(ITrainingDatasetDefinition datasetDefinition, 
            string persistedTrainingModelPath, 
            int howManySamplesToUseFromTestDataset)
        {
            using (var evaluationMinibatchSourceModel = MinibatchSource.TextFormatMinibatchSource
                (TrainingDataset.TestingDatasetPath, GetStreamConfigFrom(datasetDefinition), MinibatchSource.FullDataSweep))
            {

                Function model = Function.Load(persistedTrainingModelPath, Device);
                var imageInput = model.Arguments[0];
                var labelOutput = model.Outputs.Single(o => o.Name == ClassifierName);

                var featureStreamInfo = evaluationMinibatchSourceModel.StreamInfo(FeatureStreamName);
                var labelStreamInfo = evaluationMinibatchSourceModel.StreamInfo(LabelsStreamName);

                int batchSize = 50;
                int miscountTotal = 0, totalCount = 0;

                while (true)
                {
                    var minibatchData = evaluationMinibatchSourceModel.GetNextMinibatch((uint)batchSize, Device);
                    if (minibatchData == null || minibatchData.Count == 0)
                        break;
                    totalCount += (int)minibatchData[featureStreamInfo].numberOfSamples;

                    // expected lables are in the minibatch data.
                    var labelData = minibatchData[labelStreamInfo].data.GetDenseData<float>(labelOutput);
                    var expectedLabels = labelData.Select(l => l.IndexOf(l.Max())).ToList();

                    var inputDataMap = new Dictionary<Variable, Value>() {
                        { imageInput, minibatchData[featureStreamInfo].data }
                    };

                    var outputDataMap = new Dictionary<Variable, Value>() {
                        { labelOutput, null }
                    };

                    model.Evaluate(inputDataMap, outputDataMap, Device);
                    var outputData = outputDataMap[labelOutput].GetDenseData<float>(labelOutput);
                    var actualLabels = outputData.Select(l => l.IndexOf(l.Max())).ToList();

                    int misMatches = actualLabels.Zip(expectedLabels, (a, b) => a.Equals(b) ? 0 : 1).Sum();

                    miscountTotal += misMatches;
                    MessagePrinter.PrintMessage($"Validating Model: Total Samples = {totalCount}, Misclassify Count = {miscountTotal}");

                    if (totalCount > howManySamplesToUseFromTestDataset)
                    {
                        break;
                    }
                }

                float errorRate = 1.0F * miscountTotal / totalCount;
                MessagePrinter.PrintMessage($"Model Validation Error = {errorRate}");
            }
        }

        /// <summary>
        /// Dispose everything we gathered for this training session
        /// </summary>
        public abstract void Dispose();
    }
}
