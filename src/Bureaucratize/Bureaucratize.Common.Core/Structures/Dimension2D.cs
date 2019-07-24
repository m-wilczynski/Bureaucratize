/*
   Copyright (c) 2018 Michał Wilczyński

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Drawing;

namespace Bureaucratize.Common.Core.Structures
{
    public struct Dimension2D
    {
        public readonly int Width;
        public readonly int Height;

        public Dimension2D(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public Dimension2D GetInputSizeAsMultipliesOfFour()
        {
            return new Dimension2D(
                Width % 4 == 0
                    ? Width
                    : Width + 4 - Width % 4,
                Height % 4 == 0
                    ? Height
                    : Height + 4 - Height % 4);
        }

        public Size AsSize()
        {
            return new Size(Width, Height);
        }
    }
}
