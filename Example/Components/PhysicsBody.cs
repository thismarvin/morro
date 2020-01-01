using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Components
{
    struct PhysicsBody : IComponent
    {
        public int Speed { get; private set; }

        public PhysicsBody(int speed)
        {
            Speed = speed;
        }
    }
}
