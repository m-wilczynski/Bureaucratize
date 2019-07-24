using System.Drawing;
using Bureaucratize.MachineLearning.Console.Utils.Exceptions;

namespace Bureaucratize.MachineLearning.Console.Utils
{
    public class CntkEmnistDatasetStreamPrinter
    {
        public void PrettyPrint(CntkDatasetRow cntkDatasetRow)
        {
            if (cntkDatasetRow.ImagePixels.Length != 28*28)
                throw new InvalidEmnistDatasetFeatureLengthException();
            
            Colorful.Console.WriteLine();
            Colorful.Console.WriteLine();
            Colorful.Console.WriteLine($"===== Printing {cntkDatasetRow.Label} from {cntkDatasetRow.DatasetName} =====", Color.Orange);
            Colorful.Console.WriteLine("===============================");
            for (var i = 0; i < 28; i++)
            {
                for (var j = 0; j < 28; j++)
                {
                    var colorValue = cntkDatasetRow.ImagePixels[j * 28 + i];
                    Colorful.Console.Write("#", FromValue(colorValue));
                }

                Colorful.Console.WriteLine();
            }
        }

        private Color FromValue(int value)
        {
            if (value < 15)
                return Color.Black;
            if (value < 75)
                return Color.DimGray;
            if (value < 125)
                return Color.Gray;
            if (value < 200)
                return Color.LightGray;
            return Color.GhostWhite;
        }
    }
}
