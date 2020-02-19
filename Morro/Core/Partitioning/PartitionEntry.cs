using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    class PartitionEntry
    {
        public int ID { get; set; }
        public Rectangle Bounds { get; set; }

        public PartitionEntry(int id, Rectangle bounds)
        {
            ID = id;
            Bounds = bounds;
        }
    }
}
