using System;
using System.Drawing;
using System.Globalization;
using Bureaucratize.MachineLearning.Training.Core.Definitions;
using Bureaucratize.MachineLearning.Training.Core.NeuralNetworks;
using Bureaucratize.MachineLearning.Training.Core.Runners;
using Bureaucratize.MachineLearning.Training.DataSets.EMNIST_Digits;
using Bureaucratize.MachineLearning.Training.DataSets.EMNIST_Letters;
using CNTK;

namespace Bureaucratize.MachineLearning.Console
{
    public static class EmnistTrainingConsoleSubmodule
    {
        private const string DIGITS_CHOICE = "digits";
        private const string LETTERS_CHOICE = "letters";
        private const string UPPERCASE_LETTERS_CHOICE = "uppercase-letters";

        public static void RunSubmodule(string choice)
        {
            RunEmnistTraining(choice);
        }

        private static void RunEmnistTraining(string choice)
        {
            ITrainingDatasetDefinition datasetDefinition = null;

            switch (choice)
            {
                case LETTERS_CHOICE:
                    datasetDefinition = new EMNISTLetterDataset();
                    break;
                case DIGITS_CHOICE:
                    datasetDefinition = new EMNISTDigitDataset();
                    break;
                case UPPERCASE_LETTERS_CHOICE:
                    datasetDefinition = new EMNISTUppercaseLetterDataset();
                    break;
                default:
                    SharedConsoleCommands.InvalidCommand(choice);
                    return;
            }

            TrainingSessionStart(choice);
            var msgPrinter = new ConsolePrinter();

            var outputDir = $"./{DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)}/";
            var device = DeviceDescriptor.GPUDevice(0);
            var trainingConfiguration = new TrainingSessionConfiguration
            {
                Epochs = 200,
                DumpModelSnapshotPerEpoch = true,
                ProgressEvaluationSeverity = EvaluationSeverity.PerEpoch,
                MinibatchConfig = new MinibatchConfiguration
                {
                    MinibatchSize = 64,
                    HowManyMinibatchesPerSnapshot = (60000 / 32),
                    HowManyMinibatchesPerProgressPrint = 500,
                    DumpModelSnapshotPerMinibatch = false,
                    AsyncMinibatchSnapshot = false
                },
                PersistenceConfig = TrainingModelPersistenceConfiguration.CreateWithAllLocationsSetTo(outputDir)
            };

            msgPrinter.PrintMessage("\n" + trainingConfiguration + "\n");

            using (var runner = new ConvolutionalNeuralNetworkRunner(device, trainingConfiguration, msgPrinter))
            {
                runner.RunUsing(datasetDefinition);
            }

            EmnistTrainingDone(choice);
        }

        private static void TrainingSessionStart(string trainingChoice)
        {
            Colorful.Console.WriteLine($"\n\nEMNIST {trainingChoice} training session starts.", Color.Orange);
            Colorful.Console.WriteLine("===============================");
        }

        private static void EmnistTrainingDone(string trainingChoice)
        {
            Colorful.Console.WriteLine($"\nEMNIST {trainingChoice} training completed without errors.", Color.Orange);
        }
    }
}
