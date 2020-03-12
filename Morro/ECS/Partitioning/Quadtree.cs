using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class Quadtree<T> : Partitioner<T> where T : IPartitionable
    {
        private readonly int capacity;
        private bool divided;
        private int insertionIndex;
        private T[] objects;
        private Quadtree<T> topLeft;
        private Quadtree<T> topRight;
        private Quadtree<T> bottomRight;
        private Quadtree<T> bottomLeft;

        public Quadtree(Rectangle boundary, int capacity) : base(boundary)
        {
            this.capacity = capacity;

            Initialize();
        }

        protected override void Initialize()
        {
            objects = new T[capacity];
        }

        public override HashSet<T> Query(Rectangle bounds)
        {
            HashSet<T> result = new HashSet<T>();

            if (Boundary.EntirelyWithin(bounds))
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] == null)
                        continue;

                    result.Add(objects[i]);
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
                        result.Add(objects[i]);
                    }
                }
            }

            if (!divided)
                return result;            

            result.UnionWith(topLeft.Query(bounds));
            result.UnionWith(topRight.Query(bounds));
            result.UnionWith(bottomRight.Query(bounds));
            result.UnionWith(bottomLeft.Query(bounds));

            return result;
        }

        public override bool Insert(T entry)
        {
            if (!entry.Bounds.Intersects(Boundary))
                return false;

            if (insertionIndex < capacity)
            {
                objects[insertionIndex++] = entry;
                return true;
            }
            else
            {
                if (!divided)
                    Subdivide();

                if (topLeft.Insert(entry) || topRight.Insert(entry) || bottomRight.Insert(entry) || bottomLeft.Insert(entry))
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
            topLeft = new Quadtree<T>(new Rectangle(Boundary.X, Boundary.Y, Boundary.Width / 2, Boundary.Height / 2), capacity);
            topRight = new Quadtree<T>(new Rectangle(Boundary.X + Boundary.Width / 2, Boundary.Y, Boundary.Width / 2, Boundary.Height / 2), capacity);
            bottomRight = new Quadtree<T>(new Rectangle(Boundary.X + Boundary.Width / 2, Boundary.Y + Boundary.Height / 2, Boundary.Width / 2, Boundary.Height / 2), capacity);
            bottomLeft = new Quadtree<T>(new Rectangle(Boundary.X, Boundary.Y + Boundary.Height / 2, Boundary.Width / 2, Boundary.Height / 2), capacity);
            divided = true;
        }
    }
}
