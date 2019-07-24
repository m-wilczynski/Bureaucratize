using CNTK;

namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks.ConvolutionNeuralNetwork
{
    public class PoolingParams
    {
        public PoolingType Type { get; set; }
        public Dimension2D PoolingWindow { get; set; }
        public Stride2D Stride { get; set; }
    }
}
