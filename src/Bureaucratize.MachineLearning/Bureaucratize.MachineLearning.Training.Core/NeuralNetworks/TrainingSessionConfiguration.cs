using System.Text;
using ENV = System.Environment;

namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks
{
    public class TrainingSessionConfiguration
    {
        public ushort Epochs { get; set; }
        public MinibatchConfiguration MinibatchConfig { get; set; }
        public bool DumpModelSnapshotPerEpoch { get; set; }
        public EvaluationSeverity ProgressEvaluationSeverity { get; set; }
        public TrainingModelPersistenceConfiguration PersistenceConfig { get; set; }

        public override string ToString()
        {
            var outputBuilder = new StringBuilder();

            outputBuilder.Append($"[{nameof(TrainingSessionConfiguration)}] {ENV.NewLine}" +
                $"    {nameof(Epochs)}: {Epochs} {ENV.NewLine}" +
                $"    {nameof(DumpModelSnapshotPerEpoch)}: {DumpModelSnapshotPerEpoch} {ENV.NewLine}" +
                $"    {nameof(ProgressEvaluationSeverity)}: {ProgressEvaluationSeverity}");

            outputBuilder.Append($"{ENV.NewLine}    {MinibatchConfig.ToString().Replace("   ", "        ")}");
            outputBuilder.Append($"{ENV.NewLine}    {PersistenceConfig.ToString().Replace("   ", "        ")}");

            return outputBuilder.ToString();
        }
    }

    public enum EvaluationSeverity
    {
        AfterSession,
        PerMinibatch,
        PerEpoch
    }
}
