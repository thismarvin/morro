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
    class SQuad : HybridSystem
    {
        private IComponent[] positions;
        private IComponent[] dimensions;
        private IComponent[] transforms;
        private IComponent[] colors;

        private readonly VertexTransformColor[] vertexTransforms;

        private static readonly ShapeData squareData;

        static SQuad()
        {
            squareData = GeometryManager.GetShapeData(ShapeType.Triangle);
        }

        public SQuad(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CColor), typeof(CQuad));
            Depend(typeof(SPhysics));

            vertexTransforms = new VertexTransformColor[scene.EntityCapacity];
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];
            CTransform transform = (CTransform)transforms[entity];
            CColor color = (CColor)colors[entity];

            vertexTransforms[entity] = new VertexTransformColor(position, dimension, transform, color);
        }

        public override void DrawEntity(int entity, Camera camera)
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();
            transforms = scene.GetData<CTransform>();
            colors = scene.GetData<CColor>();

            base.Update();
        }

        public override void Draw(Camera camera)
        {
            if (Entities.Count <= 0)
                return;

            using (DynamicVertexBuffer transformsBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransformColor), vertexTransforms.Length, BufferUsage.WriteOnly))
            {
                transformsBuffer.SetData(vertexTransforms);

                Engine.Graphics.GraphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
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
