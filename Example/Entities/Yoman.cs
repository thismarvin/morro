using Example.Components;
using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Entities
{
    static class Yoman
    {
        public static IComponent[] Create(int x, int y)
        {
            return new IComponent[] 
            { 
                new Transform(x, y),
                new PhysicsBody(10)
            };
        }
    }
}
