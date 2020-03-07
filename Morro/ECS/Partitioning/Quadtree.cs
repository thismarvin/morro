using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class Quadtree : Partitioner
    {
        private readonly int capacity;
        private readonly int entityCapacity;
        private bool divided;
        private int insertionIndex;
        private QuadTreeEntry[] objects;
        private Quadtree topLeft;
        private Quadtree topRight;
        private Quadtree bottomRight;
        private Quadtree bottomLeft;

        public Quadtree(Rectangle boundary, int capacity, int entityCapacity) : base(boundary)
        {
            this.capacity = capacity;
            this.entityCapacity = entityCapacity;

            Initialize();
        }

        protected override void Initialize()
        {
            objects = new QuadTreeEntry[capacity];
        }

        public override SparseSet Query(Rectangle bounds)
        {
            SparseSet result = new SparseSet(entityCapacity);

            if (Boundary.EntirelyWithin(bounds))
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] == null)
                        continue;

                    result.Add((uint)objects[i].Entity);
                }
            }
            else
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] == null)
                        continue;

                    if (bounds.Intersects(objects[i].Bounds))
                    {
                        result.Add((uint)objects[i].Entity);
                    }
                }
            }

            if (!divided)
                return result;

            result.AddRange(topLeft.Query(bounds));
            result.AddRange(topRight.Query(bounds));
            result.AddRange(bottomRight.Query(bounds));
            result.AddRange(bottomLeft.Query(bounds));

            return result;
        }

        public override bool Insert(int entity, Rectangle bounds)
        {
            if (!bounds.Intersects(Boundary))
                return false;

            if (insertionIndex < capacity)
            {
                objects[insertionIndex++] = new QuadTreeEntry(entity, bounds);
                return true;
            }
            else
            {
                if (!divided)
                    Subdivide();

                if (topLeft.Insert(entity, bounds) || topRight.Insert(entity, bounds) || bottomRight.Insert(entity, bounds) || bottomLeft.Insert(entity, bounds))
                    return true;
            }

            return false;
        }

        public override void Clear()
        {
            if (objects == null)
                return;

            if (divided)
            {
                topLeft.Clear();
                topRight.Clear();
                bottomRight.Clear();
                bottomLeft.Clear();

                topLeft = null;
                topRight = null;
                bottomRight = null;
                bottomLeft = null;
            }

            divided = false;
            insertionIndex = 0;

            Array.Clear(objects, 0, objects.Length);
        }

        private void Subdivide()
        {
            topLeft = new Quadtree(new Rectangle(Boundary.X, Boundary.Y, Boundary.Width / 2, Boundary.Height / 2), capacity, entityCapacity);
            topRight = new Quadtree(new Rectangle(Boundary.X + Boundary.Width / 2, Boundary.Y, Boundary.Width / 2, Boundary.Height / 2), capacity, entityCapacity);
            bottomRight = new Quadtree(new Rectangle(Boundary.X + Boundary.Width / 2, Boundary.Y + Boundary.Height / 2, Boundary.Width / 2, Boundary.Height / 2), capacity, entityCapacity);
            bottomLeft = new Quadtree(new Rectangle(Boundary.X, Boundary.Y + Boundary.Height / 2, Boundary.Width / 2, Boundary.Height / 2), capacity, entityCapacity);
            divided = true;
        }

        internal sealed class QuadTreeEntry
        {
            public int Entity { get; set; }
            public Rectangle Bounds { get; set; }

            public QuadTreeEntry(int entity, Rectangle bounds)
            {
                Entity = entity;
                Bounds = bounds;
            }
        }
    }
}
