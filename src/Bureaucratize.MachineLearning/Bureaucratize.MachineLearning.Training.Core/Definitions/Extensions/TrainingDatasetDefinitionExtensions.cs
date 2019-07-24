using Bureaucratize.MachineLearning.Training.Core.Runners.Base;
using CNTK;

namespace Bureaucratize.MachineLearning.Training.Core.Definitions.Extensions
{
    public static class TrainingDatasetDefinitionExtensions
    {
        public static Variable AsInputFor(this ITrainingDatasetDefinition datasetDefinition, DeepLearningRunner inputConsumer)
        {
            return CNTKLib.InputVariable(datasetDefinition.SingleElementDimensions, DataType.Float, inputConsumer.FeatureStreamName);
        }

        public static Variable ScaledForConvolutionalNetwork(this Variable input, DeepLearningRunner inputConsumer)
        {
            return CNTKLib.ElementTimes(Constant.Scalar(0.00390625f, inputConsumer.Device), input);
        }
    }
}
