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
        private readonly VertexTransformColor[] transforms;

        private DynamicVertexBuffer transformBuffer;
        private VertexBufferBinding[] vertexBufferBindings;
        private bool dataChanged;

        public PolygonGroup(ShapeData sharedShapeData, int capacity) : base(capacity)
        {
            this.sharedShapeData = sharedShapeData;
            transforms = new VertexTransformColor[capacity];
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
                transforms[groupIndex++] = new VertexTransformColor(entry.Transform, entry.Color);
                dataChanged = true;
                return true;
            }

            return false;
        }

        private void UpdateBuffer()
        {
            transformBuffer?.Dispose();

            transformBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransformColor), transforms.Length, BufferUsage.WriteOnly);
            transformBuffer.SetData(transforms);

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(sharedShapeData.Geometry),
                new VertexBufferBinding(transformBuffer, 0, 1)
            };
        }

        public override void Draw(Camera camera)
        {
            if (dataChanged)
            {
                UpdateBuffer();
                dataChanged = false;
            }

            Engine.Graphics.GraphicsDevice.SetVertexBuffers(vertexBufferBindings);
            Engine.Graphics.GraphicsDevice.Indices = sharedShapeData.Indices;

            GeometryManager.PolygonShader.Parameters["WorldViewProjection"].SetValue(camera.World * camera.View * camera.Projection);

            foreach (EffectPass pass in GeometryManager.PolygonShader.Techniques[1].Passes)
            {
                pass.Apply();
                Engine.Graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, sharedShapeData.TotalTriangles, transforms.Length);
            }
        }
    }
}
