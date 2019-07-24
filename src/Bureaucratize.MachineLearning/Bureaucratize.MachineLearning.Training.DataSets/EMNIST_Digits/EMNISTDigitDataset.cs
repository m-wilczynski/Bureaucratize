using Bureaucratize.MachineLearning.Training.Core.Definitions;
using Bureaucratize.MachineLearning.Training.Core.NeuralNetworks;
using System.IO;

namespace Bureaucratize.MachineLearning.Training.DataSets.EMNIST_Digits
{
    public class EMNISTDigitDataset : ITrainingDatasetDefinition
    {
        public string MappingPath => @".\Resources\emnist-mnist-mapping.txt";
        public string TestImagesPath => @".\Resources\emnist-mnist-test-images-idx3-ubyte.gz";
        public string TestLabelsPath => @".\Resources\emnist-mnist-test-labels-idx1-ubyte.gz";
        public string TrainImagesPath => @".\Resources\emnist-mnist-train-images-idx3-ubyte.gz";
        public string TrainLabelsPath => @".\Resources\emnist-mnist-train-labels-idx1-ubyte.gz";
        public string OutputFileSuffix => "Digits";
        public string DataSetName => "EMNIST_Digits";
        public uint SingleElementSize => 28*28;
        public uint LabelsAmount => 10;
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
