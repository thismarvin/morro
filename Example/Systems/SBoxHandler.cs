using Example.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class SBoxHandler : MorroSystem
    {
        private readonly VertexTransform[] transforms;
        private static readonly ShapeData squareData;

        static SBoxHandler()
        {
            squareData = GeometryManager.GetShapeData(ShapeType.Square);
        }

        public SBoxHandler(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CQuad));

            transforms = new VertexTransform[scene.TotalEntities];
        }

        public override void UpdateEntity(int entity)
        {
            if (!scene.EntityInView(entity))
                return;

            CPosition position = scene.GetData<CPosition>(entity);
            CDimension dimension = scene.GetData<CDimension>(entity);
            CTransform transform = scene.GetData<CTransform>(entity);
            CQuad aabb = scene.GetData<CQuad>(entity);

            aabb.SetTransform(position, dimension, transform);

            transforms[entity] = (new VertexTransform(aabb.Transform));
        }

        public override void DrawEntity(int entity, SpriteBatch spriteBatch, Camera camera)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (Entities.Count <= 0)
                return;

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
