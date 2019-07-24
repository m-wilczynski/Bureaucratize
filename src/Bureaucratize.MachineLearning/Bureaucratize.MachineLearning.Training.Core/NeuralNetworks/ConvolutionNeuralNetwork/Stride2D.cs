namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks.ConvolutionNeuralNetwork
{
    public struct Stride2D
    {
        public int Horizontal;
        public int Vertical;

        public Stride2D(int horizontal, int vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        public int[] AsArray()
        {
            return new[] { Horizontal, Vertical };
        }
    }
}
