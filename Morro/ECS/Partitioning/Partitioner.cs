using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    public enum PartitionerType
    {
        Bin,
        Quadtree
    }

    abstract class Partitioner<T> where T : IPartitionable
    {
        public RectangleF Boundary
        {
            get => boundary;
            set
            {
                Boundary = value;
                Clear();
                Initialize();
            }
        }

        private RectangleF boundary;

        internal Partitioner(RectangleF boundary)
        {
            this.boundary = boundary;
        }

        protected abstract void Initialize();

        public abstract List<int> Query(RectangleF bounds);

        public abstract bool Insert(T entry);

        public abstract void Clear();
    }
}
