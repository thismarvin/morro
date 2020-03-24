using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    /// <summary>
    /// Provides functionality to be processed by any given <see cref="Partitioner{T}"/>.
    /// </summary>
    interface IPartitionable
    {
        int Identifier { get; set; }
        RectangleF Bounds { get; set; }
    }
}
