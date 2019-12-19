using Microsoft.Xna.Framework;
using Morro.Core;
using Morro.Graphics;
using Morro.Maths;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    public enum Integrator
    {
        Euler,
        VelocityVerlet
    }

    abstract class Kinetic : Entity
    {
        public float MoveSpeed { get; protected set; }
        public Vector2 Velocity { get; protected set; }
        public Vector2 Acceleration { get; protected set; }

        public Kinetic(float x, float y, int width, int height, float moveSpeed) : base(x, y, width, height)
        {
            MoveSpeed = moveSpeed;
        }

        protected abstract void Collision();

        protected void ApplyForce(Integrator integrator)
        {
            switch (integrator)
            {
                case Integrator.Euler:
                    SetPosition
                    (
                        Position.X + Velocity.X * Engine.DeltaTime,
                        Position.Y + Velocity.Y * Engine.DeltaTime
                    );

                    Velocity = new Vector2
                    (
                        Velocity.X + Acceleration.X * Engine.DeltaTime,
                        Velocity.Y + Acceleration.Y * Engine.DeltaTime
                    );
                    break;

                case Integrator.VelocityVerlet:
                    SetPosition
                    (
                        Position.X + Velocity.X * Engine.DeltaTime + 0.5f * Acceleration.X * Engine.DeltaTime * Engine.DeltaTime,
                        Position.Y + Velocity.Y * Engine.DeltaTime + 0.5f * Acceleration.Y * Engine.DeltaTime * Engine.DeltaTime
                    );

                    Velocity = new Vector2
                    (
                        Velocity.X + Acceleration.X * Engine.DeltaTime,
                        Velocity.Y + Acceleration.Y * Engine.DeltaTime
                    );
                    break;
            }
        }

        /// <summary>
        /// WIP. AABB collsion is fine, but everything else is weird.
        /// </summary>
        protected List<CollisionType> ResolveCollisionAgainst(Polygon polygon)
        {
            List<CollisionType> collisionTypes;

            if (polygon is AABB)
                collisionTypes = CollisionHelper.ResolveCollisionBetween(AABB, (AABB)polygon, Velocity);
            else if (polygon is RightTriangle)
                collisionTypes = CollisionHelper.ResolveCollisionBetween(AABB, (RightTriangle)polygon, Velocity);
            else
                collisionTypes = CollisionHelper.NaivelyResolveCollisionBetween(AABB, polygon, Velocity);

            SetPosition(AABB.X, AABB.Y);

            return collisionTypes;
        }
    }
}
