using System.IO;
using Bureaucratize.MachineLearning.Training.Core.Definitions;
using Bureaucratize.MachineLearning.Training.Core.NeuralNetworks;

namespace Bureaucratize.MachineLearning.Training.DataSets.EMNIST_Letters
{
    public class EMNISTUppercaseLetterDataset : ITrainingDatasetDefinition
    {
        public string MappingPath => @".\Resources\emnist-byclass-mapping-filtered.txt";
        public string TestImagesPath => @".\Resources\emnist-byclass-test-images-idx3-ubyte-filtered.gz";
        public string TestLabelsPath => @".\Resources\emnist-byclass-test-labels-idx1-ubyte-filtered.gz";
        public string TrainImagesPath => @".\Resources\emnist-byclass-train-images-idx3-ubyte-filtered.gz";
        public string TrainLabelsPath => @".\Resources\emnist-byclass-train-labels-idx1-ubyte-filtered.gz";
        public string OutputFileSuffix => "UppercaseLetters";
        public string DataSetName => "EMNIST_UppercaseLetters";
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
