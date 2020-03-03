using Morro.ECS;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class QuadUpdater : UpdateSystem
    {
        public VertexTransform[] Transforms { get; private set; }

        public QuadUpdater(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CQuad));
            Transforms = new VertexTransform[scene.TotalEntities];
        }

        public override void UpdateEntity(int entity)
        {             
            CPosition position = scene.GetData<CPosition>(entity);
            CDimension dimension = scene.GetData<CDimension>(entity);
            CTransform transform = scene.GetData<CTransform>(entity);
            CQuad quad = scene.GetData<CQuad>(entity);

            quad.SetTransform(position, dimension, transform);
            Transforms[entity] = new VertexTransform(quad.Transform);
        }
    }
}
