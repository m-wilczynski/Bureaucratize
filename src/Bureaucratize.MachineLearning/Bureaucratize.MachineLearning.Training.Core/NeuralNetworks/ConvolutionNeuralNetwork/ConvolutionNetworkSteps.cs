using CNTK;
using System.Linq;

namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks.ConvolutionNeuralNetwork
{
    public static class CNNSteps
    {
        //TODO: Make configurable
        public const double CONV_W_SCALE = 0.26;

        public static Function ConvolutionLayer(ConvolutionParams parameters, Variable features, DeviceDescriptor device)
        {
            return CNTKLib.Convolution(
                new Parameter(parameters.AsArray(), DataType.Float, CNTKLib.GlorotUniformInitializer(CONV_W_SCALE, -1, 2), device),
                features, 
                parameters.Stride.AsArray());
        }

        public static Function PoolingLayer(PoolingParams parameters, Variable convolutionFunction)
        {
            return CNTKLib.Pooling(
                convolutionFunction, 
                parameters.Type, 
                parameters.PoolingWindow.AsArray(), 
                parameters.Stride.AsArray(), 
                new[] { true });
        }

        public static Function FullyConnectedLinearLayer
            (Variable previousLayer, int numberOfOutputClasses, DeviceDescriptor device, string outputName)
        {
            //Reshape previous layer to rank 1 if needed, ie. flatten
            if (previousLayer.Shape.Rank != 1)
            {
                int newDim = previousLayer.Shape.Dimensions.Aggregate((d1, d2) => d1 * d2);
                previousLayer = CNTKLib.Reshape(previousLayer, new int[] { newDim });
            }

            int inputDim = previousLayer.Shape[0];

            //Fully connect all feature inputs (inputDim) with all outputs (numberOfOutputClasses)
            //We get a matrix of size [inputDim, numberOfOutputClasses]
            int[] timesInput = { numberOfOutputClasses, inputDim };

            //Initial weights with random, uniform values
            var timesParam = new Parameter(timesInput, DataType.Float,
                CNTKLib.GlorotUniformInitializer(
                    CNTKLib.DefaultParamInitScale,
                    CNTKLib.SentinelValueForInferParamInitRank,
                    CNTKLib.SentinelValueForInferParamInitRank, 1),
                device, "timesParam");
            var timesFunction = CNTKLib.Times(timesParam, previousLayer, "times");

            //Additional bias connected to each output
            int[] bias = { numberOfOutputClasses };
            var plusParam = new Parameter(bias, 0.0f, device, "plusParam");
            return CNTKLib.Plus(plusParam, timesFunction, outputName);
        }

        public static ConvolutionalNeuralNetworkConfiguration AppendConvolutionLayer(this ConvolutionalNeuralNetworkConfiguration config,
            ConvolutionParams parameters, DeviceDescriptor device)
        {
            return config.AppendUsingPreviousStep(prevStep => ConvolutionLayer(parameters, prevStep, device), "Convolution Layer");
        }

        public static ConvolutionalNeuralNetworkConfiguration AppendReluActivation(this ConvolutionalNeuralNetworkConfiguration config)
        {
            return config.AppendUsingPreviousStep(prevStep => CNTKLib.ReLU(prevStep), "ReLU Layer");
        }

        public static ConvolutionalNeuralNetworkConfiguration AppendTanHActivation(this ConvolutionalNeuralNetworkConfiguration config)
        {
            return config.AppendUsingPreviousStep(prevStep => CNTKLib.Tanh(prevStep), "TanH Layer");
        }

        public static ConvolutionalNeuralNetworkConfiguration AppendPoolingLayer
            (this ConvolutionalNeuralNetworkConfiguration config, PoolingParams parameters)
        {
            return config.AppendUsingPreviousStep(prevStep => PoolingLayer(parameters, prevStep), "Pooling Layer");
        }

        public static ConvolutionalNeuralNetworkConfiguration AppendFullyConnectedLinearLayer
            (this ConvolutionalNeuralNetworkConfiguration config, DeviceDescriptor device)
        {
            return config.AppendUsingPreviousStep(prevStep =>
                FullyConnectedLinearLayer(prevStep, config.NumberOfOutputClasses, device, config.OutputName), "Fully Connected Layer");
        }
    }
}
