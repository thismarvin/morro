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
            CPosition position = new CPosition(x, y);
            CDimension dimension = new CDimension(size, size);

            return new IComponent[]
            {
                position,
                dimension,
                new CAABB(position, dimension),
                new CPhysicsBody(RandomHelper.RandomVector2(RandomHelper.Range(0, 300)), new Vector2(0, 75)),                
            };
        }
    }
}
