using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Components
{
    class PhysicsBody : Component
    {
        public float Speed { get; private set; }

        public PhysicsBody(float speed) : base("PhysicsBody")
        {
            Speed = speed;
        }
    }
}
