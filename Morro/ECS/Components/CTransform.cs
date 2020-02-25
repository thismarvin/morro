using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class CTransform : IComponent
    {
        public float Rotation { get; set; }
        public Vector3 RotationOffset { get; set; }       
        public Vector3 Translation{ get; set; }
        public Vector3 Scale { get; set; }

        public CTransform()
        {
            Rotation = 0;
            RotationOffset = Vector3.Zero;
            Translation = Vector3.Zero;
            Scale = new Vector3(1);
        }
    }
}
