using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Core
{
    struct Rectangle
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public float Top { get { return Y; } }
        public float Bottom { get { return Y + Height; } }
        public float Left { get { return X; } }
        public float Right { get { return X + Width; } }

        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Intersects(Rectangle rectangle)
        {
            return (Left < rectangle.Right && Right > rectangle.Left && Top < rectangle.Bottom && Bottom > rectangle.Top);
        }

        public bool EntirelyWithin(Rectangle rectangle)
        {
            return (Left >= rectangle.Left && Right <= rectangle.Right && Top > rectangle.Top && Bottom <= rectangle.Bottom);
        }

        public override string ToString()
        {
            return base.ToString() + " " + string.Format(CultureInfo.InvariantCulture, "- [ {0}, {1} ], {2}x{3}", X, Y, Width, Height);
        }
    }
}
