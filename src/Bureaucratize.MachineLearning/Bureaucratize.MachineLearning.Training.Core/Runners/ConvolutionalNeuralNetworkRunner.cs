using Bureaucratize.MachineLearning.Training.Core.Definitions;
using Bureaucratize.MachineLearning.Training.Core.Definitions.Extensions;
using Bureaucratize.MachineLearning.Training.Core.NeuralNetworks;
using Bureaucratize.MachineLearning.Training.Core.NeuralNetworks.ConvolutionNeuralNetwork;
using Bureaucratize.MachineLearning.Training.Core.Runners.Base;
using Bureaucratize.MachineLearning.Training.Core.Runners.Output;
using CNTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bureaucratize.MachineLearning.Training.Core.Runners
{
    public class ConvolutionalNeuralNetworkRunner : DeepLearningRunner
    {
        public ConvolutionalNeuralNetworkRunner(DeviceDescriptor device, TrainingSessionConfiguration configuration, IMessagePrinter printer) 
            : base(device, configuration, printer)
        {
        }

        public ConvolutionalNeuralNetworkConfiguration NetworkConfiguration { get; private set; }

        private Function _networkClassifier = null;
        private Variable _input = null;
        private Variable _labels = null;
        private Function _trainingLossFunction = null;
        private Function _evaluationFunction = null;
        private Trainer _trainer = null;
        private MinibatchSource _minibatchSource;
        private StreamInformation _featureStreamInfo;
        private StreamInformation _labelStreamInfo;

        protected override void CleanUp()
        {
            foreach (var disposable in new IDisposable[]
                {
                    NetworkConfiguration,
                    _networkClassifier,
                    _input,
                    _labels,
                    _trainingLossFunction,
                    _evaluationFunction,
                    _trainer,
                    _minibatchSource,
                    _featureStreamInfo,
                    _labelStreamInfo
                }.Where(d => d != null))
            {
                disposable.Dispose();
            }
        }

        protected override void BuildNeuralNetwork(ITrainingDatasetDefinition datasetDefinition)
        {
            _input = datasetDefinition.AsInputFor(this);

            TrainingDataset = datasetDefinition.BuildDatasetIfNotPresent();
            NetworkConfiguration = new ConvolutionalNeuralNetworkConfiguration(
                    _input.ScaledForConvolutionalNetwork(this), 
                    (int)datasetDefinition.LabelsAmount, 
                    ClassifierName);

            //INPUT -> CONV -> RELU -> MAX POOL -> CONV -> RELU -> MAX POOL -> FC

            //28x28x1 -> 14x14x4
            NetworkConfiguration.AppendConvolutionLayer(
                new ConvolutionParams
                {
                    FilterSize = new Dimension2D(3, 3),
                    Channels = 1,
                    OutputFeatureMapsCount = 4,
                    Stride = new Stride3D(1, 1, 1)
                }, Device);

            NetworkConfiguration.AppendReluActivation();
            NetworkConfiguration.AppendPoolingLayer(
                new PoolingParams
                {
                    Type = PoolingType.Max,
                    PoolingWindow = new Dimension2D(3, 3),
                    Stride = new Stride2D(2, 2)
                });

            //14x14x4 -> 7x7x8
            NetworkConfiguration.AppendConvolutionLayer(
                new ConvolutionParams
                {
                    FilterSize = new Dimension2D(3, 3),
                    Channels = 4,
                    OutputFeatureMapsCount = 8,
                    Stride = new Stride3D(1, 1, 4)
                }, Device);

            NetworkConfiguration.AppendTanHActivation();
            NetworkConfiguration.AppendPoolingLayer(
                new PoolingParams
                {
                    Type = PoolingType.Max,
                    PoolingWindow = new Dimension2D(3, 3),
                    Stride = new Stride2D(2, 2)
                });

            //Fully Connect
            NetworkConfiguration.AppendFullyConnectedLinearLayer(Device);

            _networkClassifier = NetworkConfiguration.Evaluate();

            MessagePrinter.PrintMessage(NetworkConfiguration.ToString());

            _labels = CNTKLib.InputVariable(new int[] { (int)datasetDefinition.LabelsAmount }, DataType.Float, LabelsStreamName);
            _trainingLossFunction = CNTKLib.CrossEntropyWithSoftmax(new Variable(_networkClassifier), _labels, LossFunctionName);
            _evaluationFunction = CNTKLib.ClassificationError(new Variable(_networkClassifier), _labels, ClassificationErrorName);

        }

        protected override void PrepareTrainingData(ITrainingDatasetDefinition datasetDefinition)
        {
            _minibatchSource = MinibatchSource.TextFormatMinibatchSource(
                TrainingDataset.TrainingDatasetPath, GetStreamConfigFrom(datasetDefinition), MinibatchSource.InfinitelyRepeat);

            _featureStreamInfo = _minibatchSource.StreamInfo(FeatureStreamName);
            _labelStreamInfo = _minibatchSource.StreamInfo(LabelsStreamName);
        }

        protected override void ConfigureTrainer()
        {
            //TODO: Should I dispose learners too or CNTK disposes them already inside Trainer?
            _trainer = Trainer.CreateTrainer(_networkClassifier, _trainingLossFunction, _evaluationFunction,
                new List<Learner>() {
                    Learner.SGDLearner(_networkClassifier.Parameters(), new TrainingParameterScheduleDouble(0.0003125, 1))
                }
            );
        }

        protected override void TrainNetwork(ITrainingDatasetDefinition datasetDefinition)
        {
            int i = 0;
            ushort currentEpoch = Configuration.Epochs;

            while (currentEpoch > 0)
            {
                var minibatchData = _minibatchSource.GetNextMinibatch(Configuration.MinibatchConfig.MinibatchSize, Device);
                var arguments = new Dictionary<Variable, MinibatchData>
                {
                    { _input, minibatchData[_featureStreamInfo] },
                    { _labels, minibatchData[_labelStreamInfo] }
                };

                _trainer.TrainMinibatch(arguments, Device);

                PrintProgress(i++);
                
                if (minibatchData.Values.Any(batchData => batchData.sweepEnd))
                {
                    currentEpoch--;
                    if (Configuration.DumpModelSnapshotPerEpoch)
                    {
                        _networkClassifier.Save(
                            Configuration.PersistenceConfig.GetEpochFileNamePathFor(Convert.ToUInt16(Configuration.Epochs - currentEpoch),
                            datasetDefinition));
                    }
                }
            }
        }

        protected override void SaveResults(ITrainingDatasetDefinition datasetDefinition)
        {
            _networkClassifier.Save(Configuration.PersistenceConfig.GetTrainingResultFileNamePathFor(datasetDefinition));
        }

        public override void Dispose()
        {
            CleanUp();
        }

        private void PrintProgress(int minibatchNumber)
        {
            if (_trainer == null || Configuration?.MinibatchConfig == null)
                throw new InvalidOperationException();

            if ((minibatchNumber % Configuration.MinibatchConfig.HowManyMinibatchesPerProgressPrint) == 0 && _trainer.PreviousMinibatchSampleCount() != 0)
            {
                double trainLossValue = _trainer.PreviousMinibatchLossAverage();
                double evaluationValue = _trainer.PreviousMinibatchEvaluationAverage();
                MessagePrinter.PrintMessage($"Minibatch: {minibatchNumber} CrossEntropyLoss = {trainLossValue}, EvaluationCriterion = {evaluationValue}");
            }
        }
    }
}
