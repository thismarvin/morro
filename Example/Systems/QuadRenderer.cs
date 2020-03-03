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

        public override void DrawEntity(int entity, SpriteBatch spriteBatch, Camera camera)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (Entities.Count <= 0)
                return;

            VertexTransform[] transforms = new VertexTransform[Entities.Count];
            int transformIndex = 0;
            foreach (int entity in Entities)
            {
                transforms[transformIndex++] = new VertexTransform(scene.GetData<CQuad>(entity).Transform);
            }

            using (DynamicVertexBuffer transformsBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransform), transforms.Length, BufferUsage.WriteOnly))
            {
                transformsBuffer.SetData(transforms);

                spriteBatch.GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(squareData.Geometry), new VertexBufferBinding(transformsBuffer, 0, 1));
                spriteBatch.GraphicsDevice.Indices = squareData.Indices;

                GeometryManager.SetupPolygonShader(camera);

                foreach (EffectPass pass in GeometryManager.PolygonShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, squareData.TotalTriangles, transforms.Length);
                }
            }
        }
    }
}
