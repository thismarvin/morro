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
        private IComponent[] aabbs;

        private readonly List<VertexTransform> transforms;
        private static readonly Effect polygonShader;

        public SBoxHandler(Scene scene) : base(scene)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CAABB));

            transforms = new List<VertexTransform>();
        }

        static SBoxHandler()
        {
            AssetManager.LoadEffect("PolygonShader", "Assets/Effects/Polygon");
            polygonShader = AssetManager.GetEffect("PolygonShader").Clone();
        }

        public override void BeforeUpdate()
        {
            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();
            aabbs = scene.GetData<CAABB>();

            transforms.Clear();
        }

        public override void UpdateEntity(int entity)
        {
            if (!scene.EntityInView(entity))
                return;

            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];
            CAABB aabb = (CAABB)aabbs[entity];

            aabb.SetTransform(position, dimension);

            transforms.Add(new VertexTransform(aabb.Transform));
        }

        public override void BeforeDraw(SpriteBatch spriteBatch)
        {
                        
        }

        public override void DrawEntity(int entity, SpriteBatch spriteBatch, Camera camera)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (transforms.Count <= 0)
                return;

            using (DynamicVertexBuffer transformsBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransform), transforms.Count, BufferUsage.WriteOnly))
            {
                transformsBuffer.SetData(transforms.ToArray());

                spriteBatch.GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(Geometry.Shapes.Get("Square").Geometry), new VertexBufferBinding(transformsBuffer, 0, 1));
                spriteBatch.GraphicsDevice.Indices = Geometry.Shapes.Get("Square").Indices;

                polygonShader.Parameters["WorldViewProjection"].SetValue(camera.World * camera.View * camera.Projection);

                foreach (EffectPass pass in polygonShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 2, transforms.Count);
                }
            }
        }
    }
}
