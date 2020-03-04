using Example.Components;
using Microsoft.Xna.Framework;
using Morro.ECS;
using Morro.Maths;

namespace Example.Entities
{
    static class Yoman
    {
        private static readonly Color[] palette = { Color.White, Color.Red, Color.Orange, Color.Blue };

        public static IComponent[] Create(float x, float y, float size)
        {
            CPosition position = new CPosition(x, y);
            CDimension dimension = new CDimension(size, size);
            CTransform transform = new CTransform()
            {
                Rotation = (float)Random.Range(0, System.Math.PI * 2),
                RotationOffset = new Vector2(size / 2, size / 2),
            };

            return new IComponent[]
            {
                position,
                dimension,
                transform,
                new CColor(palette[Random.Range(0, palette.Length - 1)]),
                new CQuad(),
                new CPhysicsBody(Random.RandomVector2(Random.Range(0, 300)), new Vector2(0, 75)),
            };
        }
    }
}
