using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Core
{
    abstract class MonoObject : IComparable
    {
        public Vector2 Position { get; private set; }
        public Rectangle Bounds { get; private set; }

        public float X { get { return Position.X; } }
        public float Y { get { return Position.Y; } }
        public int Width { get { return (int)Bounds.Width; } }
        public int Height { get { return (int)Bounds.Height; } }
        public Vector2 Center { get { return new Vector2(Position.X + Width / 2, Position.Y + Height / 2); } }

        public int Depth { get; set; }

        public MonoObject(float x, float y, int width, int height)
        {
            Position = new Vector2(x, y);
            Bounds = new Rectangle(X, Y, width, height);

            Depth = 1;
        }

        public virtual void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
            Bounds = new Rectangle(X, Y, Width, Height);
        }

        public virtual void SetBounds(float x, float y, int width, int height)
        {
            Position = new Vector2(x, y);
            Bounds = new Rectangle(X, Y, width, height);
        }

        public void SetWidth(int width)
        {
            SetDimensions(width, Height);
        }

        public void SetHeight(int height)
        {
            SetDimensions(Width, height);
        }

        public virtual void SetDimensions(int width, int height)
        {
            Bounds = new Rectangle(X, Y, width, height);
        }

        public int CompareTo(object obj)
        {
            return Depth.CompareTo(((MonoObject)obj).Depth);
        }

        public override string ToString()
        {
            return base.ToString() + string.Format(CultureInfo.InvariantCulture, ": Position:(X:{0}, Y:{1}), Dimensions:(W:{2}, H:{3})", X, Y, Width, Height);
        }
    }
}
