    using Bureaucratize.MachineLearning.Training.Core.Definitions;
using Bureaucratize.MachineLearning.Training.Core.NeuralNetworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Bureaucratize.MachineLearning.Training.DataSets
{
    public static class EMNISTParser
    {
        public static readonly string EMNISTTrainFilenameMask = "EMNIST_Train-CNTK_{0}.txt";
        public static readonly string EMNISTTestFilenameMask = "EMNIST_Test-CNTK_{0}.txt";

        public static PreparedLearningDataset ParseFromGZipedDefinitionsForCntk(ITrainingDatasetDefinition datasetDefinition)
        {
            var output = new PreparedLearningDataset();

            output.ValueToLabelMap = BuildEMNISTValueToLabelMapFor(datasetDefinition);
            output.TrainingDatasetPath = ReadEMNISTInCntkFormat(datasetDefinition, output.ValueToLabelMap, true);
            output.TestingDatasetPath = ReadEMNISTInCntkFormat(datasetDefinition, output.ValueToLabelMap, false);

            return output;
        }

        public static string ReadEMNISTInCntkFormat(ITrainingDatasetDefinition definition, SortedDictionary<byte, char> valueToLabel, bool isTrainData)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            if (valueToLabel == null)
            {
                throw new ArgumentNullException(nameof(valueToLabel));
            }

            var outputFilePath = isTrainData ?
                string.Format(EMNISTTrainFilenameMask, definition.OutputFileSuffix) :
                string.Format(EMNISTTestFilenameMask, definition.OutputFileSuffix);

            using (var imagesGzip = new GZipStream(
                File.Open(isTrainData ? definition.TrainImagesPath : definition.TestImagesPath, FileMode.Open), 
                CompressionMode.Decompress))
            using (var labelsGzip = new GZipStream(
                File.Open(isTrainData ? definition.TrainLabelsPath : definition.TestLabelsPath, FileMode.Open), 
                CompressionMode.Decompress))
            {
                using (var imagesReader = new BinaryReader(imagesGzip, Encoding.ASCII))
                using (var labelsReader = new BinaryReader(labelsGzip, Encoding.ASCII))
                {
                    int magicNumber1 = imagesReader.ReadBigEndianInt32();
                    int numImages = imagesReader.ReadBigEndianInt32();
                    int numRows = imagesReader.ReadBigEndianInt32();
                    int numCols = imagesReader.ReadBigEndianInt32();

                    int magicNumber2 = labelsReader.ReadBigEndianInt32();
                    int numLabels = labelsReader.ReadBigEndianInt32();

                    using (var writer = new StreamWriter(outputFilePath))
                    {
                        for (var i = 0; i < numImages; i++)
                        {
                            writer.Write(Environment.NewLine
                                + $"|labels {labelsReader.ReadByte().AsCntkLabelDefinition(valueToLabel)} "
                                +  "|features");
                            for (var j = 0; j < 28 * 28; j++)
                            {
                                writer.Write(" ");
                                writer.Write(imagesReader.ReadByte());
                            }
                        }
                    }
                }
            }

            return outputFilePath;
        }

        public static SortedDictionary<byte, char> BuildEMNISTValueToLabelMapFor(ITrainingDatasetDefinition definition)
        {
            var output = new SortedDictionary<byte, char>();

            using (var mappingReader = new StreamReader(File.Open(definition.MappingPath, FileMode.Open)))
            {
                var line = "";
                while ((line = mappingReader.ReadLine()) != null)
                {
                    var split = line.Split(' ');
                    output.Add(byte.Parse(split[0]), (char)byte.Parse(split[1]));
                }
            }
            return output;
        }

        public static void ReadAndPersistEMNISTDatasetFilteredOut(string imagesPath, string labelsPath, 
            Func<byte, bool> filter, Func<byte, byte> labelRemapper)
        {
            var targetDirectory = Path.GetDirectoryName(imagesPath);
            var imagesFilteredFileName = Path.GetFileName(imagesPath).Split('.')[0] + "-filtered.gz";
            var labelsFilteredFileName = Path.GetFileName(labelsPath).Split('.')[0] + "-filtered.gz";
            const int imageSize = 28 * 28;

            using (var imagesGzip = new GZipStream(
                File.Open(imagesPath, FileMode.Open),
                CompressionMode.Decompress))
            using (var labelsGzip = new GZipStream(
                File.Open(labelsPath, FileMode.Open),
                CompressionMode.Decompress))
            {
                using (var imagesReader = new BinaryReader(imagesGzip, Encoding.ASCII))
                using (var labelsReader = new BinaryReader(labelsGzip, Encoding.ASCII))
                {
                    int magicNumber1 = imagesReader.ReadBigEndianInt32();
                    int numImages = imagesReader.ReadBigEndianInt32();
                    int numRows = imagesReader.ReadBigEndianInt32();
                    int numCols = imagesReader.ReadBigEndianInt32();

                    int magicNumber2 = labelsReader.ReadBigEndianInt32();
                    int numLabels = labelsReader.ReadBigEndianInt32();


                    int filteredOutImagesCount = 0;

                    using (var imagesFilteredGzip = new GZipStream(
                        File.Open(Path.Combine(targetDirectory, imagesFilteredFileName), FileMode.Create),
                        CompressionMode.Compress))
                    using (var labelsFilteredGzip = new GZipStream(
                        File.Open(Path.Combine(targetDirectory, labelsFilteredFileName), FileMode.Create),
                        CompressionMode.Compress))
                    {

                        using (var imagesWriter = new BinaryWriter(imagesFilteredGzip))
                        using (var labelsWriter = new BinaryWriter(labelsFilteredGzip))
                        {
                            byte[] imagesToWrite = new byte[numImages * 28 * 28];
                            byte[] labelsToWrite = new byte[numLabels];

                            for (var i = 0; i < numImages; i++)
                            {
                                var label = labelsReader.ReadByte();
                                var shouldAccept = filter(label);

                                if (shouldAccept)
                                {
                                    labelsToWrite[filteredOutImagesCount] = label;
                                    for (var j = 0; j < imageSize; j++)
                                    {
                                        imagesToWrite[(filteredOutImagesCount * imageSize) + j] = imagesReader.ReadByte();
                                    }

                                    filteredOutImagesCount++;
                                }
                                else
                                {
                                    for (var j = 0; j < imageSize; j++)
                                    {
                                        imagesReader.ReadByte();
                                    }
                                }
                            }

                            imagesWriter.Write(magicNumber1);
                            imagesWriter.WriteInBigEndian(filteredOutImagesCount);
                            imagesWriter.Write(28);
                            imagesWriter.Write(28);

                            labelsWriter.Write(magicNumber2);
                            labelsWriter.WriteInBigEndian(filteredOutImagesCount);

                            for (var i = 0; i < filteredOutImagesCount; i++)
                            {
                                labelsWriter.Write(labelRemapper(labelsToWrite[i]));

                                for (var j = 0; j < imageSize; j++)
                                {
                                    var byteToWrite = imagesToWrite[(i * imageSize) + j];
                                    imagesWriter.Write(byteToWrite);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string AsCntkLabelDefinition(this byte valueToFormat, SortedDictionary<byte, char> valueToLabel)
        {
            if (valueToLabel == null)
            {
                throw new ArgumentNullException(nameof(valueToLabel));
            }

            byte minValue = valueToLabel.Keys.First(), maxValue = valueToLabel.Keys.Last();

            //TODO: Possibly faster with fixed int[maxLabelValue] array, 
            //where you set only [valueToFormat-1] element to 1 and aggregate all as one string?
            string output = "";
            for (byte i = minValue; i <= maxValue; i++)
            {
                if (i != minValue)
                {
                    output += " ";
                }

                if (i == valueToFormat)
                {
                    output += "1";
                }
                else
                {
                    output += "0";
                }
            }

            return output;
        }

        private static int ReadBigEndianInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(Int32));
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        private static void WriteInBigEndian(this BinaryWriter bw, int integer)
        {
            var bytes = BitConverter.GetBytes(integer);
            Array.Reverse(bytes);
            bw.Write(bytes);
        }
    }
}
