using Morro.ECS;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class QuadUpdater : UpdateSystem
    {
        public VertexTransform[] VertexTransforms { get; private set; }

        private IComponent[] positions;
        private IComponent[] dimensions;
        private IComponent[] transforms;
        private IComponent[] quads;

        public QuadUpdater(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CQuad));
            VertexTransforms = new VertexTransform[scene.TotalEntities];
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];
            CTransform transform = (CTransform)transforms[entity];
            CQuad quad = (CQuad)quads[entity];

            quad.SetTransform(position, dimension, transform);
            VertexTransforms[entity] = new VertexTransform(quad.Transform);
        }

        public override void Update()
        {
            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();
            transforms = scene.GetData<CTransform>();
            quads = scene.GetData<CQuad>();

            base.Update();
        }
    }
}
