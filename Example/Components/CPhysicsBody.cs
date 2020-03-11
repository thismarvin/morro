using Microsoft.Xna.Framework;
using Morro.Core;
using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Components
{
    class CPhysicsBody : IComponent
    {
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public float Accumulator { get; set; }
        public TimeSpan LastUpdate { get; set; }

        public CPhysicsBody()
        {
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;

            LastUpdate = new TimeSpan(Engine.TotalGameTime.Ticks);
        }

        public CPhysicsBody(Vector2 initialVelocity, Vector2 initialAcceleration)
        {
            Velocity = initialVelocity;
            Acceleration = initialAcceleration;

            LastUpdate = new TimeSpan(Engine.TotalGameTime.Ticks);
        }
    }
}
