namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks.ConvolutionNeuralNetwork
{
    public class ConvolutionParams
    {
        public Dimension2D FilterSize { get; set; }
        public int Channels { get; set; }
        public int OutputFeatureMapsCount { get; set; }
        public Stride3D Stride { get; set; }

        public int[] AsArray()
        {
            return new[]
            {
                FilterSize.Width,
                FilterSize.Height,
                Channels,
                OutputFeatureMapsCount
            };
        }
    }
}