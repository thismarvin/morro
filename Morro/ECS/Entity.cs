using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    abstract class Entity : MonoObject
    {
        public AABB AABB { get; private set; }
        public bool Remove { get; set; }

        public Entity(float x, float y, int width, int height) : base(x, y, width, height)
        {
            AABB = new AABB(X, Y, Width, Height, 2, Color.Red, VertexInformation.Dynamic);
        }

        public override void SetPosition(float x, float y)
        {
            if (X == x && Y == y)
                return;

            base.SetPosition(x, y);
            AABB.SetPosition(X, Y);
        }

        public override void SetBounds(float x, float y, int width, int height)
        {
            base.SetBounds(x, y, width, height);
            AABB.SetBounds(X, Y, Width, Height);
        }

        public override void SetDimensions(int width, int height)
        {
            base.SetDimensions(width, height);
            AABB.SetDimensions(Width, Height);
        }

        public Core.Rectangle GetQueryingBounds(int buffer)
        {
            return new Core.Rectangle(
                X - buffer,
                Y - buffer,
                Width + buffer * 2,
                Height + buffer * 2
            );
        }

        public abstract void Update();
        public abstract void Draw(SpriteBatch spriteBatch, Camera camera);
    }
}
