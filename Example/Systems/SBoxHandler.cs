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
        private IComponent[] positions;
        private IComponent[] dimensions;
        private IComponent[] cTransforms;
        private IComponent[] quads;

        private readonly VertexTransform[] transforms;
        private static readonly Effect polygonShader;
        private static readonly ShapeData squareData;

        static SBoxHandler()
        {
            polygonShader = AssetManager.GetEffect("PolygonShader").Clone();
            squareData = GeometryManager.GetShapeData(ShapeType.Square);
        }

        public SBoxHandler(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CQuad));

            transforms = new VertexTransform[scene.TotalEntities];
        }

        public override void BeforeUpdate()
        {
            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();
            cTransforms = scene.GetData<CTransform>();
            quads = scene.GetData<CQuad>();
        }

        public override void UpdateEntity(int entity)
        {
            if (!scene.EntityInView(entity))
                return;

            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];
            CTransform transform = (CTransform)cTransforms[entity];
            CQuad aabb = (CQuad)quads[entity];

            aabb.SetTransform(position, dimension, transform);

            transforms[entity] = (new VertexTransform(aabb.Transform));
        }

        public override void BeforeDraw(SpriteBatch spriteBatch)
        {
                        
        }

        public override void DrawEntity(int entity, SpriteBatch spriteBatch, Camera camera)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (transforms.Length <= 0)
                return;

            using (DynamicVertexBuffer transformsBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransform), transforms.Length, BufferUsage.WriteOnly))
            {
                transformsBuffer.SetData(transforms);

                spriteBatch.GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(squareData.Geometry), new VertexBufferBinding(transformsBuffer, 0, 1));
                spriteBatch.GraphicsDevice.Indices = squareData.Indices;

                polygonShader.Parameters["WorldViewProjection"].SetValue(camera.World * camera.View * camera.Projection);

                foreach (EffectPass pass in polygonShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, squareData.TotalTriangles, transforms.Length);
                }
            }
        }
    }
}
