using Microsoft.Xna.Framework;
using Morro.ECS;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Components
{
    class CBoid : IComponent
    {
        public float ViewRadius { get; set; }
        public float MoveSpeed { get; set; }
        public float MaxForce { get; set; }
        public float Angle { get; set; }

        public CBoid()
        {

        }

        public CBoid(float viewRadius, float moveSpeed, float maxForce, float angle)
        {
            ViewRadius = viewRadius;
            MoveSpeed = moveSpeed;
            MaxForce = maxForce;
            Angle = angle;
        }
    }
}
