using Bureaucratize.MachineLearning.Training.Core.Runners.Output;
using System;
using System.Drawing;

namespace Bureaucratize.MachineLearning.Console
{
    public class ConsolePrinter : IMessagePrinter
    {
        public void PrintMessage(string message)
        {
            foreach (var line in message
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (line.Contains("    "))
                {
                    Colorful.Console.WriteLine(line, Color.Gray);
                }
                else
                {
                    Colorful.Console.WriteLine(line);
                }
            }
        }
    }
}
