using System.Drawing;
using Bureaucratize.MachineLearning.Training.DataSets;

namespace Bureaucratize.MachineLearning.Console
{
    public class TrainingStart
    {
        private const string DIGITS_CHOICE = "digits";
        private const string LETTERS_CHOICE = "letters";
        private const string UPPERCASE_LETTERS_CHOICE = "uppercase-letters";
        private const string PRETTY_PRINT_CHOICE = "prettyprint";
        private const string EXIT_CHOICE = "exit";

        public static void Main()
        {
            Colorful.Console.SetWindowSize(110, 30);
            Colorful.Console.BackgroundColor = Color.Black;
            Colorful.Console.ForegroundColor = Color.GhostWhite;

            SayHello();

            while (true)
            {
                PrintOptions();

                string input = Colorful.Console.ReadLine();

                switch (input)
                {
                    case EXIT_CHOICE:
                        goto exit;
                    case PRETTY_PRINT_CHOICE:
                        PrettyPrintConsoleSubmodule.RunSubmodule();
                        continue;
                    case LETTERS_CHOICE:
                    case DIGITS_CHOICE:
                    case UPPERCASE_LETTERS_CHOICE:
                        EmnistTrainingConsoleSubmodule.RunSubmodule(input);
                        break;
                    default:
                        SharedConsoleCommands.InvalidCommand(input);
                        continue;
                }
            }

            exit:
            {
                Colorful.Console.WriteLine("Press key to close...", Color.Yellow);
                Colorful.Console.ReadKey();
            }
        }

        private static void SayHello()
        {
            Colorful.Console.WriteAscii("BUREAUCRATIZE");

            Colorful.Console.WriteLine("EMNIST Model training using CNN", Color.Yellow);
            Colorful.Console.WriteLine("===============================");
        }

        private static void PrintOptions()
        {
            Colorful.Console.WriteLine("\nMAIN MODULE", Color.Orange);
            Colorful.Console.WriteLine("\nOptions:");
            Colorful.Console.WriteLine($"    '{DIGITS_CHOICE}' - will train model for digits from EMNIST dataset", Color.Gray);
            Colorful.Console.WriteLine($"    '{LETTERS_CHOICE}' - will train model for letters from EMNIST dataset", Color.Gray);
            Colorful.Console.WriteLine($"    '{UPPERCASE_LETTERS_CHOICE}' - will train model for uppercase letters from EMNIST dataset", Color.Gray);
            Colorful.Console.WriteLine($"    '{PRETTY_PRINT_CHOICE}' - will pretty print pasted features of one " +
                                             $"EMNIST dataset feature line", Color.Gray);
            Colorful.Console.WriteLine($"    '{EXIT_CHOICE}' - will terminate application", Color.Gray);
            SharedConsoleCommands.YourInput();
        }

        private static void TransformByClassDatasetToUppercaseLettersDataset()
        {
            EMNISTParser.ReadAndPersistEMNISTDatasetFilteredOut(
                @".\Resources\emnist-byclass-test-images-idx3-ubyte.gz",
                @".\Resources\emnist-byclass-test-labels-idx1-ubyte.gz",
                label => label > 9 && label < 36,
                label => (byte)(label - 10));

            EMNISTParser.ReadAndPersistEMNISTDatasetFilteredOut(
                @".\Resources\emnist-byclass-train-images-idx3-ubyte.gz",
                @".\Resources\emnist-byclass-train-labels-idx1-ubyte.gz",
                label => label > 9 && label < 36,
                label => (byte)(label - 10));
        }
    }

    public static class SharedConsoleCommands
    {
        public static void InvalidCommand(string input)
        {
            Colorful.Console.WriteLine($"Command '{input}' not recognized. Printing possible options for you...", Color.IndianRed);
        }

        public static void YourInput()
        {
            Colorful.Console.WriteLine("\n[Your input]", Color.DimGray);
        }
    }
}
