using ENV = System.Environment;

namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks
{
    public class MinibatchConfiguration
    {
        public ushort MinibatchSize { get; set; }
        public ushort HowManyMinibatchesPerSnapshot { get; set; }
        public ushort HowManyMinibatchesPerProgressPrint { get; set; }
        public bool DumpModelSnapshotPerMinibatch { get; set; }
        public bool AsyncMinibatchSnapshot { get; set; }

        public override string ToString()
        {
            return $"[{nameof(MinibatchConfiguration)}] {ENV.NewLine}" +
                    $"    {nameof(MinibatchSize)}: {MinibatchSize} {ENV.NewLine}" +
                    $"    {nameof(HowManyMinibatchesPerSnapshot)}: {HowManyMinibatchesPerSnapshot} {ENV.NewLine}" +
                    $"    {nameof(HowManyMinibatchesPerProgressPrint)}: {HowManyMinibatchesPerProgressPrint} {ENV.NewLine}" +
                    $"    {nameof(DumpModelSnapshotPerMinibatch)}: {DumpModelSnapshotPerMinibatch} {ENV.NewLine}" +
                    $"    {nameof(AsyncMinibatchSnapshot)}: {AsyncMinibatchSnapshot}";
        }
    }
}
