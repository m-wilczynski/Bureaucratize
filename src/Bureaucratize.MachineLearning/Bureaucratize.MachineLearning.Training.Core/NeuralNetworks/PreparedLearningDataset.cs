using System.Collections.Generic;

namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks
{
    public class PreparedLearningDataset
    {
        public string TrainingDatasetPath { get;set; }
        public string TestingDatasetPath { get; set; }
        public SortedDictionary<byte, char> ValueToLabelMap { get; set; }
    }
}
