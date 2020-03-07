using Morro.Utilities;
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

        public HashSet<MorroObject> Query(MorroObject morroObject)
        {
            return Query(morroObject, 0);
        }

        public HashSet<MorroObject> Query(MorroObject morroObject, int buffer)
        {
            return Query(new Rectangle(morroObject.X - buffer, morroObject.Y - buffer, morroObject.Width + buffer * 2, morroObject.Height + buffer * 2));
        }

        protected abstract void Initialize();

        public abstract HashSet<MorroObject> Query(Rectangle bounds);

        public abstract bool Insert(MorroObject morroObject);

        public abstract void Clear();
    }
}
