using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Example.Systems
{
    class QuadRenderer : DrawSystem
    {
        private static readonly ShapeData squareData;

        static QuadRenderer()
        {
            squareData = GeometryManager.GetShapeData(ShapeType.Square);
        }

        public QuadRenderer(Scene scene) : base(scene)
        {
            Require(typeof(CQuad));
        }

        public override void DrawEntity(int entity, Camera camera)
        {
            throw new NotImplementedException();
        }

        public override void Draw(Camera camera)
        {
            if (Entities.Count <= 0)
                return;

            VertexTransformColor[] transforms = scene.GetSystem<QuadUpdater>().VertexTransforms;

            using (DynamicVertexBuffer transformsBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransformColor), transforms.Length, BufferUsage.WriteOnly))
            {
                transformsBuffer.SetData(transforms);

                Engine.Graphics.GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(squareData.Geometry), new VertexBufferBinding(transformsBuffer, 0, 1));
                Engine.Graphics.GraphicsDevice.Indices = squareData.Indices;

                GeometryManager.SetupPolygonShader(camera);

                foreach (EffectPass pass in GeometryManager.PolygonShader.Techniques[1].Passes)
                {
                    pass.Apply();
                    Engine.Graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, squareData.TotalTriangles, transforms.Length);
                }
            }
        }
    }
}
