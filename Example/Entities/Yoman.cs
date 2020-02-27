using Example.Components;
using Microsoft.Xna.Framework;
using Morro.ECS;
using Morro.Maths;

namespace Example.Entities
{
    static class Yoman
    {
        public static IComponent[] Create(float x, float y, float size)
        {
            CPosition position = new CPosition(x, y);
            CDimension dimension = new CDimension(size, size);
            CTransform transform = new CTransform()
            {
                Rotation = (float)Random.Range(0, System.Math.PI * 2),
                RotationOffset = new Vector3(size / 2, size / 2, 0),
            };

            return new IComponent[]
            {
                position,
                dimension,
                transform,
                new CQuad(position, dimension, transform),
                new CPhysicsBody(Random.RandomVector2(Random.Range(0, 300)), new Vector2(0, 75)),
            };
        }
    }
}
