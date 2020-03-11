using Example.Components;
using Morro.ECS;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Entities
{
    static class Boid
    {
        public static IComponent[] Create(float x, float y)
        {
            CBoid boid = new CBoid(16, Morro.Maths.Random.Range(40, 50), 0.5f, 0);
            CPhysicsBody physicsBody = new CPhysicsBody(Morro.Maths.Random.RandomVector2(boid.MoveSpeed), Microsoft.Xna.Framework.Vector2.Zero);
            CPosition position = new CPosition(x, y);
            CDimension dimension = new CDimension(2, 2);
            CTransform transform = new CTransform()
            {
                Scale = new Microsoft.Xna.Framework.Vector3(2, 1, 1),
                Rotation = -(float)Math.Atan2(physicsBody.Velocity.Y, physicsBody.Velocity.X),
                RotationOffset = new Microsoft.Xna.Framework.Vector2(dimension.Width / 2, dimension.Height / 2)
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
                new CQuad(),
                new CPartitionable(),
            };
        }
    }
}
