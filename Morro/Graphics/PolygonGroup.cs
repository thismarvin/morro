using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class PolygonGroup
    {
        public const int MaximumCapacity = 100000;

        public ShapeData SharedShapeData { get; private set; }
        public int Capacity { get; private set; }

        private readonly VertexTransform[] transforms;
        private int polygonIndex;

        private static readonly Effect polygonShader;

        static PolygonGroup()
        {
            polygonShader = AssetManager.GetEffect("PolygonShader").Clone();
        }

        public PolygonGroup(ShapeData sharedShapeData, int capacity)
        {
            SharedShapeData = sharedShapeData;
            Capacity = capacity > MaximumCapacity ? MaximumCapacity : capacity;
            transforms = new VertexTransform[Capacity];
        }

        public bool Add(MPolygon polygon)
        {
            if (polygonIndex >= Capacity)
                return false;

            if (polygon.ShapeData == SharedShapeData)
            {
                transforms[polygonIndex++] = new VertexTransform(polygon.Transform);
                return true;
            }

            return false;
        }

        public void Clear()
        {
            Array.Clear(transforms, 0, transforms.Length);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            using (DynamicVertexBuffer transformsBuffer = new DynamicVertexBuffer(spriteBatch.GraphicsDevice, typeof(VertexTransform), transforms.Length, BufferUsage.WriteOnly))
            {
                transformsBuffer.SetData(transforms);

                spriteBatch.GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(SharedShapeData.Geometry), new VertexBufferBinding(transformsBuffer, 0, 1));
                spriteBatch.GraphicsDevice.Indices = SharedShapeData.Indices;

                polygonShader.Parameters["WorldViewProjection"].SetValue(camera.World * camera.View * camera.Projection);

                foreach (EffectPass pass in polygonShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, SharedShapeData.TotalTriangles, transforms.Length);
                }
            }
        }
    }
}
