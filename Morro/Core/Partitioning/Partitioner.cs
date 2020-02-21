using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public enum PartitionerType
    {
        None,
        Quadtree,
        Bin
    }

    abstract class Partitioner
    {
        public Rectangle Boundary { get; private set; }

        public Partitioner(Rectangle boundary)
        {
            Boundary = boundary;
        }

        public void SetBoundary(Rectangle boundary)
        {
            Boundary = boundary;

            Clear();
            Initialize();
        }

        protected abstract void Initialize();

        public abstract List<PartitionEntry> Query(Rectangle bounds);

        public abstract bool Insert(PartitionEntry entry);

        public abstract void Clear();
    }
}
