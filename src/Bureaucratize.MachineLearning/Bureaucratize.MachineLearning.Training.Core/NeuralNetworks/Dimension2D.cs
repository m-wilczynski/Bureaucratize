namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks
{
    public struct Dimension2D
    {
        public int Width;
        public int Height;

        public Dimension2D(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int[] AsArray()
        {
            return new[] { Width, Height };
        }
    }
}
