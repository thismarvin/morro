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
    static class Hawk
    {
        public static IComponent[] Create(float x, float y)
        {
            CBoid boid = new CBoid()
            {
                ViewRadius = 16,
                MoveSpeed = MoreRandom.Range(60, 80),
                MaxForce = 0.4f
            };
            CKinetic physicsBody = new CKinetic(MoreRandom.RandomVector2(boid.MoveSpeed), Vector2.Zero);
            CPosition position = new CPosition(x, y);
            CDimension dimension = new CDimension(4, 4);
            CTransform transform = new CTransform()
            {
                Scale = new Vector3(3, 1, 1),
                Rotation = -(float)Math.Atan2(physicsBody.Velocity.Y, physicsBody.Velocity.X),
                RotationOffset = new Vector2(dimension.Width / 2, dimension.Height / 2)
            };
            CColor color = new CColor(PICO8.MidnightBlack);

            return new IComponent[]
            {
                boid,
                physicsBody,
                position,
                dimension,
                transform,
                color,
                new CTriangle(),
                new CPartitionable(),
                new CPredator()
            };
        }
    }
}
