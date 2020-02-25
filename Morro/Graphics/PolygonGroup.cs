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
        private readonly ShapeData sharedShapeData;
        private readonly VertexTransform[] transforms;
        private int polygonIndex;

        private static readonly int capacity;
        private static readonly Effect polygonShader;

        static PolygonGroup()
        {
            polygonShader = AssetManager.GetEffect("PolygonShader").Clone();
            capacity = 100000;
        }

        public PolygonGroup(ShapeData sharedShapeData)
        {
            this.sharedShapeData = sharedShapeData;
            transforms = new VertexTransform[capacity];
        }

        public bool Add(MPolygon polygon)
        {
            if (polygonIndex >= capacity)
                return false;

            if (polygon.ShapeData == sharedShapeData)
            {
                transforms[polygonIndex++] = new VertexTransform(polygon.Transform);
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            using (DynamicVertexBuffer transformsBuffer = new DynamicVertexBuffer(spriteBatch.GraphicsDevice, typeof(VertexTransform), transforms.Length, BufferUsage.WriteOnly))
            {
                transformsBuffer.SetData(transforms);

                spriteBatch.GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(sharedShapeData.Geometry), new VertexBufferBinding(transformsBuffer, 0, 1));
                spriteBatch.GraphicsDevice.Indices = sharedShapeData.Indices;

                polygonShader.Parameters["WorldViewProjection"].SetValue(camera.World * camera.View * camera.Projection);

                foreach (EffectPass pass in polygonShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, sharedShapeData.TotalTriangles, transforms.Length);
                }
            }
        }
    }
}
