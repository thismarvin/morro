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
        public Vector2 Velocity { get; private set; }
        public Vector2 Acceleration { get; private set; }
        public float Accumulator { get; set; }
        public TimeSpan LastUpdate { get; set; }

        public CPhysicsBody(Vector2 initialVelocity, Vector2 initialAcceleration)
        {
            Velocity = initialVelocity;
            Acceleration = initialAcceleration;

            LastUpdate = new TimeSpan(Engine.TotalGameTime.Ticks);
        }

        public void SetVelocity(float x, float y)
        {
            Velocity = new Vector2(x, y);
        }

        public void SetAcceleration(float x, float y)
        {
            Acceleration = new Vector2(x, y);
        }
    }
}
