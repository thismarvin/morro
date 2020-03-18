using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    interface IPartitionable
    {
        int Identifier { get; set; }
        Core.Rectangle Bounds { get; set; }
    }
}
