using Example.Components;
using Microsoft.Xna.Framework;
using Morro.ECS;
using Morro.Graphics;
using Morro.Graphics.Palettes;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Entities
{
    static class Boid
    {
        public static IComponent[] Create(float x, float y)
        {
            CBoid boid = new CBoid()
            {
                ViewRadius = 16,
                MoveSpeed = MoreRandom.Range(40, 50),
                MaxForce = 0.5f
            };
            CKinetic kinetic = new CKinetic(MoreRandom.RandomVector2(boid.MoveSpeed), Vector2.Zero);
            CPosition position = new CPosition(x, y);
            CDimension dimension = new CDimension(2, 2);
            CTransform transform = new CTransform()
            {
                Scale = new Vector3(2, 1, 1),
                Rotation = -(float)Math.Atan2(kinetic.Velocity.Y, kinetic.Velocity.X),
                RotationOffset = new Vector2(dimension.Width / 2, dimension.Height / 2)
            };
            CColor color = new CColor(PICO8.MidnightBlack);

            return new IComponent[]
            {
                boid,
                kinetic,
                position,
                dimension,
                transform,
                color,
                new CTriangle(),
                new CPartitionable(),
            };
        }
    }
}
