using Example.Components;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class STriangle : SimpleShapeSystem
    {
        public STriangle(Scene scene) : base(scene, GeometryManager.GetShapeData(ShapeType.Triangle), typeof(CTriangle), 4)
        {
            Depend(typeof(SPhysics));
        }
    }
}
