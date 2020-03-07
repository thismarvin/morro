using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    abstract class Partitioner
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

        public SparseSet Query(CPosition position, CDimension dimension)
        {
            return Query(position, dimension, 0);
        }

        public SparseSet Query(CPosition position, CDimension dimension, int buffer)
        {
            return Query(new Rectangle(position.X - buffer, position.Y - buffer, dimension.Width + buffer * 2, dimension.Height + buffer * 2));
        }

        protected abstract void Initialize();

        public abstract SparseSet Query(Rectangle bounds);

        public abstract bool Insert(int entity, Rectangle bounds);

        public abstract void Clear();
    }
}
