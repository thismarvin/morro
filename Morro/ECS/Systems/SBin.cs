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

        internal SBin(Scene scene, Bin bin) : base(scene, 4, 120)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CPartitionable));

            this.bin = bin;
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

            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();

            bin.Clear();
            base.Update();
            scene.FinalizePartition();
        }
    }
}
