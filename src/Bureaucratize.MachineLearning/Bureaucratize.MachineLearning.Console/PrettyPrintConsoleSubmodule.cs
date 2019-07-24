using System;
using System.Drawing;
using Bureaucratize.MachineLearning.Console.Utils;
using Bureaucratize.MachineLearning.Console.Utils.Exceptions;
using Bureaucratize.MachineLearning.Training.Core.Definitions;
using Bureaucratize.MachineLearning.Training.DataSets.EMNIST_Digits;
using Bureaucratize.MachineLearning.Training.DataSets.EMNIST_Letters;

namespace Bureaucratize.MachineLearning.Console
{
    public static class PrettyPrintConsoleSubmodule
    {
        private const string DIGITS_CHOICE = "digits";
        private const string LETTERS_CHOICE = "letters";
        private const string UPPERCASE_LETTERS_CHOICE = "uppercase-letters";
        private const string EXIT_CHOICE = "exit";

        public static void RunSubmodule()
        {
            PrettyPrint();
        }

        private static void PrettyPrint()
        {
            while (true)
            {
                PrintPrettyPrintOptions();

                SharedConsoleCommands.YourInput();
                var input = Colorful.Console.ReadLine();
                ITrainingDatasetDefinition datasetDefinition;
                switch (input)
                {
                    case EXIT_CHOICE:
                        return;
                    case DIGITS_CHOICE:
                        datasetDefinition = new EMNISTDigitDataset();
                        break;
                    case LETTERS_CHOICE:
                        datasetDefinition = new EMNISTLetterDataset();
                        break;
                    case UPPERCASE_LETTERS_CHOICE:
                        datasetDefinition = new EMNISTUppercaseLetterDataset();
                        break;
                    default:
                        SharedConsoleCommands.InvalidCommand(input);
                        continue;
                }

                try
                {
                    Colorful.Console.WriteLine("Choose line:", Color.Gray);
                    CntkDatasetRow prettyPrintInput = null;
                    try
                    {
                        var inputLineNum = Int32.Parse(Colorful.Console.ReadLine());
                        prettyPrintInput =
                            new CntkEmnistDatasetStreamFetcher().GetRowFromDefinition(datasetDefinition, inputLineNum);
                    }
                    catch (Exception ex) when (ex is ArgumentNullException ||  ex is FormatException)
                    {
                        Colorful.Console.WriteLine($"Not a valid line number!", Color.IndianRed);
                        continue;
                    }
                    new CntkEmnistDatasetStreamPrinter().PrettyPrint(prettyPrintInput);
                }
                catch (InvalidEmnistDatasetFeatureLengthException)
                {
                    Colorful.Console.WriteLine($"Feature stream is not of length 28*28. Cannot pretty print it.", Color.Gray);
                }
            }
        }

        private static void PrintPrettyPrintOptions()
        {
            Colorful.Console.WriteLine();
            Colorful.Console.WriteLine("PRETTY PRINT MODULE", Color.Orange);
            Colorful.Console.WriteLine("\nOptions:");
            Colorful.Console.WriteLine($"    '{DIGITS_CHOICE}' - will use EMNIST MNIST digits dataset", Color.Gray);
            Colorful.Console.WriteLine($"    '{LETTERS_CHOICE}' - will use EMNIST balanced letters dataset", Color.Gray);
            Colorful.Console.WriteLine($"    '{UPPERCASE_LETTERS_CHOICE}' - will use EMNIST filtered, unbalanced, uppercase letters dataset", Color.Gray);
            Colorful.Console.WriteLine($"    '{EXIT_CHOICE}' - will exit pretty print mode", Color.Gray);
        }
    }
}
