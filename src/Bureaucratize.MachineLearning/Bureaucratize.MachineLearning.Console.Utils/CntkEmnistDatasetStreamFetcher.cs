using System;
using System.IO;
using System.Linq;
using Bureaucratize.MachineLearning.Training.Core.Definitions;
using Bureaucratize.MachineLearning.Training.Core.NeuralNetworks;

namespace Bureaucratize.MachineLearning.Console.Utils
{
    public class CntkEmnistDatasetStreamFetcher
    {
        public CntkDatasetRow GetRowFromDefinition(ITrainingDatasetDefinition datasetDefinition, int rowNumber)
        {
            var dataset = datasetDefinition.BuildDatasetIfNotPresent();

            int rowRead = 0;
            using (var stream = new FileStream(dataset.TrainingDatasetPath, 
                                                FileMode.Open, 
                                                FileAccess.Read, 
                                                FileShare.Read))
            using (var reader = new StreamReader(stream))
            {
                string line = null;
                while (rowRead != rowNumber)
                {
                    if (reader.EndOfStream) return null;
                    line = reader.ReadLine();
                    rowRead++;
                }

                return ParseLineAsCntkDatasetRow(line, datasetDefinition, dataset);
            }
        }

        private CntkDatasetRow ParseLineAsCntkDatasetRow(string readLine, 
            ITrainingDatasetDefinition datasetDefinition, PreparedLearningDataset dataset)
        {
            var rowStreams = readLine.Split('|').Skip(1).ToList();
            var labelStream = GetDatasetStreamItems(rowStreams[0], "labels");

            return new CntkDatasetRow
            {
                DatasetName = datasetDefinition.DataSetName,
                ImagePixels = GetDatasetStreamItems(rowStreams[1], "features"),
                Label = CntkLabelFromLabelStream(labelStream, dataset)
            };
        }

        private static int[] GetDatasetStreamItems(string streamLine, string streamLabel)
        {
            return streamLine.Split(' ')
                .Where(el => !string.IsNullOrWhiteSpace(el) && el != streamLabel)
                .Select(int.Parse)
                .ToArray();
        }

        private static char CntkLabelFromLabelStream(int[] labelStream, PreparedLearningDataset preparedDataset)
        {
            byte counter = preparedDataset.ValueToLabelMap.Keys.OrderBy(k => k).FirstOrDefault();

            foreach (var item in labelStream)
            {
                if (item == 1)
                {
                    return preparedDataset.ValueToLabelMap[counter];
                }
                counter++;
            }

            throw new InvalidOperationException();
        }
    }

    public class CntkDatasetRow
    {
        public string DatasetName { get; set; }
        public char Label { get; set; }
        public int[] ImagePixels { get; set; }
    }
}

