using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class SBin : UpdateSystem
    {
        private IComponent[] positions;
        private IComponent[] dimensions;

        private readonly Bin bin;
        private readonly float target;
        private float accumulator;

        internal SBin(Scene scene, Bin bin) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CPartitionable));

            this.bin = bin;
            target = 1 / 120f;
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];

            bin.Insert(entity, new Rectangle(position.X, position.Y, (int)dimension.Width, (int)dimension.Height));
        }

        public override void Update()
        {
            if (!scene.PartitioningEnabled)
                return;

            bin.Clear();
            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();

            accumulator += Engine.DeltaTime;
            while (accumulator >= target)
            {
                base.Update();
                accumulator -= target;
            }

            scene.FinalizePartition();            
        }
    }
}
