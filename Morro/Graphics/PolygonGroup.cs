using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class PolygonGroup : DrawGroup<MPolygon>
    {
        private readonly ShapeData sharedShapeData;
        private readonly VertexTransform[] transforms;

        public PolygonGroup(ShapeData sharedShapeData, int capacity) : base(capacity)
        {
            this.sharedShapeData = sharedShapeData;
            transforms = new VertexTransform[capacity];
            group = null;
        }

        protected override bool ConditionToAdd(MPolygon polygon)
        {
            return polygon.ShapeData == sharedShapeData;
        }

        public override bool Add(MPolygon entry)
        {
            if (groupIndex >= capacity)
                return false;

            if (ConditionToAdd(entry))
            {
                transforms[groupIndex++] = new VertexTransform(entry.Transform);
                return true;
            }

            return false;
        }

        public override void Draw(Camera camera)
        {
            using (DynamicVertexBuffer transformsBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransform), transforms.Length, BufferUsage.WriteOnly))
            {
                transformsBuffer.SetData(transforms);

                Engine.Graphics.GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(sharedShapeData.Geometry), new VertexBufferBinding(transformsBuffer, 0, 1));
                Engine.Graphics.GraphicsDevice.Indices = sharedShapeData.Indices;

                GeometryManager.PolygonShader.Parameters["WorldViewProjection"].SetValue(camera.World * camera.View * camera.Projection);

                foreach (EffectPass pass in GeometryManager.PolygonShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Engine.Graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, sharedShapeData.TotalTriangles, transforms.Length);
                }
            }
        }
    }
}
