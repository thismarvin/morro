using Example.Components;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class SQuad : SimpleShapeSystem
    {
        public SQuad(Scene scene) : base(scene, GeometryManager.GetShapeData(ShapeType.Square), typeof(CQuad), 4)
        {
            Depend(typeof(SPhysics));
        }
    }
}
