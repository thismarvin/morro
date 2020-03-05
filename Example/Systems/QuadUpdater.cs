using Microsoft.Xna.Framework;
using Morro.ECS;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class QuadUpdater : UpdateSystem
    {
        public VertexTransformColor[] VertexTransforms { get; private set; }

        private IComponent[] positions;
        private IComponent[] dimensions;
        private IComponent[] transforms;
        private IComponent[] colors;

        public QuadUpdater(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CColor), typeof(CQuad));
            VertexTransforms = new VertexTransformColor[scene.EntityCapacity];
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];
            CTransform transform = (CTransform)transforms[entity];
            CColor color = (CColor)colors[entity];

            VertexTransforms[entity] = new VertexTransformColor(position, dimension, transform, color);
        }

        public override void Update()
        {
            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();
            transforms = scene.GetData<CTransform>();
            colors = scene.GetData<CColor>();

            base.Update();
        }
    }
}
