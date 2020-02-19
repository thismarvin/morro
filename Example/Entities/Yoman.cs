using Example.Components;
using Microsoft.Xna.Framework;
using Morro.ECS;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Entities
{
    static class Yoman
    {
        public static IComponent[] Create(float x, float y, float size)
        {
            return new IComponent[]
            {
                new CPosition(x, y),
                new CDimension(size, size),
                new CPhysicsBody(RandomHelper.RandomVector2(RandomHelper.Range(0, 300)), new Vector2(0, 75)),
                new CBox(x, y, (int)size)
        };
        }
    }
}
