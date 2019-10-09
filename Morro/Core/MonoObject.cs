using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    abstract class MonoObject : IComparable
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int LayerDepth { get; set; }
        public bool Remove { get; set; }
        public Color Color { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Center { get; private set; }
        public Rectangle Bounds { get; private set; }

        public MonoObject(float x, float y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            LayerDepth = 1;
            Color = Color.White;
            Position = new Vector2(X, Y);
            Center = new Vector2(X + Width / 2, Y + Height / 2);
            Bounds = new Rectangle(X, Y, Width, Height);
        }

        public virtual void SetLocation(float x, float y)
        {
            X = x;
            Y = y;
            Position = new Vector2(X, Y);
            Center = new Vector2(X + Width / 2, Y + Height / 2);
            Bounds = new Rectangle(X, Y, Width, Height);
        }

        public virtual void SetCenter(float x, float y)
        {
            SetLocation(x - Width / 2, y - Height / 2);
        }

        public virtual void SetBounds(float x, float y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Position = new Vector2(X, Y);
            Center = new Vector2(X + Width / 2, Y + Height / 2);
            Bounds = new Rectangle(X, Y, Width, Height);
        }

        public virtual void SetWidth(int width)
        {
            SetDimensions(width, Height);
        }

        public virtual void SetHeight(int height)
        {
            SetDimensions(Width, height);
        }

        public virtual void SetDimensions(int width, int height)
        {
            Width = width;
            Height = height;
            Center = new Vector2(X + Width / 2, Y + Height / 2);
            Bounds = new Rectangle(X, Y, Width, Height);
        }

        public virtual void SetColor(Color color)
        {
            Color = color;
        }

        public int CompareTo(object obj)
        {
            return LayerDepth.CompareTo(((MonoObject)obj).LayerDepth);
        }

        public override string ToString()
        {
            return base.ToString() + " " + string.Format("- [ {0}, {1} ], {2}x{3}", X, Y, Width, Height);
        }
    }
}
