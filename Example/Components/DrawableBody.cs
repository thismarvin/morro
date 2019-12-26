using Microsoft.Xna.Framework;
using Morro.ECS;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Components
{
    class DrawableBody : Component
    {
        public AABB Body { get; private set; }

        public DrawableBody() : base("DrawableBody")
        {
             Body = new AABB(-1000, -1000, 16, 16, Color.Black, Morro.Core.VertexInformation.Static);
        }

        public void SetPosition(float x, float y)
        {
            Body.SetPosition(x, y);
        }
    }
}
