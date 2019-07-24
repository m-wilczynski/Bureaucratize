namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks
{
    public struct Dimension2DChanneled
    {
        public int Width;
        public int Height;
        public int Channels;

        public int[] AsArray()
        {
            return new[] { Width, Height, Channels };
        }
    }
}
