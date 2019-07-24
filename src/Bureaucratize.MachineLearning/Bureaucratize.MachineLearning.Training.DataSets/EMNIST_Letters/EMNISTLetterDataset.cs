using Bureaucratize.MachineLearning.Training.Core.Definitions;
using Bureaucratize.MachineLearning.Training.Core.NeuralNetworks;
using System.IO;

namespace Bureaucratize.MachineLearning.Training.DataSets.EMNIST_Letters
{
    public class EMNISTLetterDataset : ITrainingDatasetDefinition
    {
        public string MappingPath => @".\Resources\emnist-letters-mapping.txt";
        public string TestImagesPath => @".\Resources\emnist-letters-test-images-idx3-ubyte.gz";
        public string TestLabelsPath => @".\Resources\emnist-letters-test-labels-idx1-ubyte.gz";
        public string TrainImagesPath => @".\Resources\emnist-letters-train-images-idx3-ubyte.gz";
        public string TrainLabelsPath => @".\Resources\emnist-letters-train-labels-idx1-ubyte.gz";
        public string OutputFileSuffix => "Letters";
        public string DataSetName => "EMNIST_Letters";
        public uint SingleElementSize => 28 * 28;
        public uint LabelsAmount => 26;
        public int[] SingleElementDimensions => new[] { 28, 28, 1 };

        public PreparedLearningDataset BuildDatasetIfNotPresent()
        {
            if (!File.Exists(string.Format(EMNISTParser.EMNISTTestFilenameMask, OutputFileSuffix)) ||
                !File.Exists(string.Format(EMNISTParser.EMNISTTrainFilenameMask, OutputFileSuffix)))
            {
                return EMNISTParser.ParseFromGZipedDefinitionsForCntk(this);
            }
            return new PreparedLearningDataset
            {
                TestingDatasetPath = string.Format(EMNISTParser.EMNISTTestFilenameMask, OutputFileSuffix),
                TrainingDatasetPath = string.Format(EMNISTParser.EMNISTTrainFilenameMask, OutputFileSuffix),
                ValueToLabelMap = EMNISTParser.BuildEMNISTValueToLabelMapFor(this)
            };
        }
    }
}
