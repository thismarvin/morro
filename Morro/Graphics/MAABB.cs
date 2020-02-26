using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class MAABB
    {
        private readonly MPolygon aabb;

        public MAABB(float x, float y, float width, float height, Color color)
        {
            aabb = new MPolygon(x, y, width, height, ShapeType.Square)
            {
                Color = color
            };
        }

        public void SetBounds(float x, float y, float width, float height)
        {
            aabb.X = x;
            aabb.Y = y;
            aabb.Width = width;
            aabb.Height = height;
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            aabb.Draw(spriteBatch, camera);
        }
    }
}
