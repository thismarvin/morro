using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class SBinPartitioner : PartitioningSystem
    {
        public SBinPartitioner(Scene scene, int maximumDimension, int targetFPS) : base(scene, targetFPS)
        {
            int optimalBinSize = (int)Math.Ceiling(Math.Log(maximumDimension, 2));
            partitioner = new Bin<PartitionerEntry>(scene.SceneBounds, optimalBinSize);
        }
    }
}
