using Microsoft.Xna.Framework;
using Morro.Core;
using Morro.Graphics;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    struct CBoxCollider : IComponent
    {
 
    }

    static class CBoxColliderHelper
    {
        public static ShapeData ShapeData { get; private set; }

        static CBoxColliderHelper()
        {
            ShapeData = GeometryManager.GetShapeData(ShapeType.Square);
        }       
    }
}
