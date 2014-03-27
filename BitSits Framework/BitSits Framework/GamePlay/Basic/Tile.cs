using System;
using Microsoft.Xna.Framework;

namespace BitSits_Framework
{
    struct Tile
    {
        public const int Width = 100;
        public const int Height = Width;

        public static Rectangle GetBounds(Vector2 position)
        {
            int y = (int)Math.Floor(position.Y / Height);

            // Hex packing
            //if (y % 2 == 0) position.X -= Width / 2;
            //int x = (int)Math.Floor(position.X / Width);
            //return new Rectangle(x * Width + (y % 2 == 0 ? Width / 2 : 0), y * Height, Width, Height);

            int x = (int)Math.Floor(position.X / Width);
            return new Rectangle(x * Width, y * Height, Width, Height);
        }
    }
}
