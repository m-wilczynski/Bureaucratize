using CNTK;
using System;
using System.Collections.Generic;
using System.Linq;
using ENV = System.Environment;

namespace Bureaucratize.MachineLearning.Training.Core.NeuralNetworks.ConvolutionNeuralNetwork
{
    public class ConvolutionalNeuralNetworkConfiguration : IDisposable
    {
        //Define ordering of layers, ie. 
        //INPUT - >(CONV + RELU) -> POOL -> (CONV + RELU) -> POOL -> (RELU + FC) etc.
        //Where:
        // - INPUT - input image
        // - CONV - convolution layer
        // - RELU - ReLU activation func
        // - POOL - pooling layer
        // - FC - fully-connected layer

        private readonly ICollection<Function> _steps = new List<Function>();
        private readonly ICollection<string> _stepNames = new List<string>();
        private bool _locked = false;

        public ConvolutionalNeuralNetworkConfiguration(Variable networkInput, int numberOfOutputClasses, string outputName)
        {
            if (networkInput == null)
                throw new ArgumentNullException(nameof(networkInput));
            if (numberOfOutputClasses < 2)
                throw new ArgumentOutOfRangeException(nameof(numberOfOutputClasses));
            if (string.IsNullOrWhiteSpace(outputName))
                throw new ArgumentException("message", nameof(outputName));

            AddStep(networkInput, "Input");
            NumberOfOutputClasses = numberOfOutputClasses;
            OutputName = outputName;
        }

        public Function LastStep => _steps.Reverse().FirstOrDefault();
        public int NumberOfOutputClasses { get; }
        public string OutputName { get; }

        public ConvolutionalNeuralNetworkConfiguration AddStep(Function step, string stepName)
        {
            if (_locked) return this;
            _steps.Add(step);
            _stepNames.Add(stepName);
            return this;
        }

        public ConvolutionalNeuralNetworkConfiguration AppendUsingPreviousStep(
            Func<Function, Function> stepProducer, string stepName)
        {
            AddStep(stepProducer(LastStep), stepName);
            return this;
        }

        public void Dispose()
        {
            if (_steps != null && _steps.Any())
            {
                foreach (var step in _steps.Where(s => s != null))
                {
                    step.Dispose();
                }
            }
        }

        public Function Evaluate()
        {
            _locked = true;
            return LastStep;
        }

        public override string ToString()
        {
            return $"[{nameof(ConvolutionalNeuralNetworkConfiguration)} - Steps] {ENV.NewLine}    " +
                _stepNames.Aggregate((prev, curr) => prev + ENV.NewLine + "    " + curr) + ENV.NewLine;
        }
    }
}
