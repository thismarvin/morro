using Microsoft.Xna.Framework;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Components
{
    class CBox : IComponent
    {
        public AABB AABB { get; private set; }

        public CBox(float x, float y, int size)
        {
            AABB = new AABB(x, y, size, size, Color.Black, VertexInformation.Static);
        }
    }
}
