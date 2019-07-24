using Bureaucratize.MachineLearning.Training.Core.Definitions;
using System;
using ENV = System.Environment;

namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks
{
    public class TrainingModelPersistenceConfiguration
    {
        public string MinibatchSnapshotTargetLocation { get; set; }
        public string EpochSnapshotTargetLocation { get; set; }
        public string TrainingResultTargetLocation { get; set; }

        public string GetMinibatchFileNamePathFor(
            ushort minibatchNumber, ITrainingDatasetDefinition dataset, string customPrefix = null)
        {
            return $"{MinibatchSnapshotTargetLocation}{customPrefix}MB{minibatchNumber.ToString("000000")}_{dataset.DataSetName}.Model";
        }

        public string GetEpochFileNamePathFor(
            ushort epochNumber, ITrainingDatasetDefinition dataset, string customPrefix = null)
        {
            return $"{EpochSnapshotTargetLocation}{customPrefix}EP{epochNumber.ToString("000000")}_{dataset.DataSetName}.Model";
        }

        public string GetTrainingResultFileNamePathFor(ITrainingDatasetDefinition dataset, string customPrefix = null)
        {
            return $"{TrainingResultTargetLocation}{customPrefix}FINAL_{dataset.DataSetName}.Model";
        }

        public static TrainingModelPersistenceConfiguration CreateWithAllLocationsSetTo(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            return new TrainingModelPersistenceConfiguration
            {
                MinibatchSnapshotTargetLocation = path,
                EpochSnapshotTargetLocation = path,
                TrainingResultTargetLocation = path
            };
        }

        public override string ToString()
        {
            return $"[{nameof(TrainingModelPersistenceConfiguration)}] {ENV.NewLine}" +
                $"    {nameof(MinibatchSnapshotTargetLocation)}: {MinibatchSnapshotTargetLocation} {ENV.NewLine}" +
                $"    {nameof(EpochSnapshotTargetLocation)}: {EpochSnapshotTargetLocation} {ENV.NewLine}" +
                $"    {nameof(TrainingResultTargetLocation)}: {TrainingResultTargetLocation}";
        }
    }
}
