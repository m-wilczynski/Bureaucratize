namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks.ConvolutionNeuralNetwork
{
    public struct Stride3D
    {
        public int Horizontal;
        public int Vertical;
        public int Channels;

        public Stride3D(int horizontal, int vertical, int channels)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            Channels = channels;
        }

        public int[] AsArray()
        {
            return new[] { Horizontal, Vertical, Channels };
        }
    }
}
