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
        SemiImplictEuler,
        VelocityVerlet
    }

    abstract class Kinetic : Entity
    {
        public float MoveSpeed { get; protected set; }
        public Vector2 Velocity { get; protected set; }
        public Vector2 Acceleration { get; protected set; }

        double accumulator;
        readonly float target;

        public Kinetic(float x, float y, int width, int height, float moveSpeed) : base(x, y, width, height)
        {
            MoveSpeed = moveSpeed;

            target = 1f / 120;
        }

        protected abstract void Collision();

        protected void ApplyForce(Integrator integrator, float deltaTime)
        {
            switch (integrator)
            {
                case Integrator.SemiImplictEuler:
                    Velocity = new Vector2
                    (
                        Velocity.X + Acceleration.X * deltaTime,
                        Velocity.Y + Acceleration.Y * deltaTime
                    );

                    SetPosition
                    (
                        Position.X + Velocity.X * deltaTime,
                        Position.Y + Velocity.Y * deltaTime
                    );
                    break;

                case Integrator.VelocityVerlet:
                    SetPosition
                    (
                        Position.X + Velocity.X * deltaTime + 0.5f * Acceleration.X * deltaTime * deltaTime,
                        Position.Y + Velocity.Y * deltaTime + 0.5f * Acceleration.Y * deltaTime * deltaTime
                    );
                    Velocity = new Vector2
                    (
                        Velocity.X + Acceleration.X * deltaTime,
                        Velocity.Y + Acceleration.Y * deltaTime
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

        private void Simultate(float elapsedTime)
        {
            accumulator += elapsedTime;

            while (accumulator >= target)
            {
                ApplyForce(Integrator.VelocityVerlet, target);
                Collision();

                accumulator -= target;
            }
        }

        public override void Update()
        {
            Simultate(Engine.DeltaTime);
        }
    }
}
