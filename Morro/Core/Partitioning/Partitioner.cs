using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public enum PartitionerType
    {
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

        public List<MonoObject> Query(MonoObject monoObject)
        {
            return Query(monoObject.Bounds);
        }

        protected abstract void Initialize();

        public abstract List<MonoObject> Query(Rectangle bounds);

        public abstract bool Insert(MonoObject monoObject);

        public abstract void Clear();
    }
}
