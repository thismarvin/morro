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
        public abstract List<MonoObject> Query(Rectangle bounds);

        public abstract bool Insert(MonoObject monoObject);

        public abstract void Clear();

        public List<MonoObject> Query(MonoObject monoObject)
        {
            return Query(monoObject.Bounds);
        }
    }
}
