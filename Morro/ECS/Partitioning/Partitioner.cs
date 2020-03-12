using Morro.Core;
using Morro.Utilities;
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
        public Rectangle Boundary
        {
            get => boundary;
            set
            {
                Boundary = value;
                Clear();
                Initialize();
            }
        }

        private Rectangle boundary;

        internal Partitioner(Rectangle boundary)
        {
            this.boundary = boundary;
        }

        protected abstract void Initialize();

        public abstract List<int> Query(Rectangle bounds);

        public abstract bool Insert(T entry);

        public abstract void Clear();
    }
}
