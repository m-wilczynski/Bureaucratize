using Bureaucratize.MachineLearning.Training.Core.NeuralNetworks;

namespace Bureaucratize.MachineLearning.Training.Core.Definitions
{
    public interface ITrainingDatasetDefinition
    {
        string MappingPath { get; }
        string TestImagesPath { get; }
        string TestLabelsPath { get; }
        string TrainImagesPath { get; }
        string TrainLabelsPath { get; }
        uint SingleElementSize { get; }
        int[] SingleElementDimensions { get; }
        uint LabelsAmount { get; }

        string OutputFileSuffix { get; }
        string DataSetName { get; }

        PreparedLearningDataset BuildDatasetIfNotPresent();
    }
}
