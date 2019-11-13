using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    class Quadtree : Partitioner
    {
        private readonly int capacity;
        private bool divided;
        private int insertionIndex;
        private Rectangle boundary;
        private readonly MonoObject[] objects;
        private Quadtree topLeft;
        private Quadtree topRight;
        private Quadtree bottomRight;
        private Quadtree bottomLeft;

        public Quadtree(Rectangle boundary, int capacity)
        {
            this.boundary = boundary;
            this.capacity = capacity;
            objects = new MonoObject[this.capacity];
        }

        public override List<MonoObject> Query(Rectangle bounds)
        {
            List<MonoObject> result = new List<MonoObject>();

            if (boundary.EntirelyWithin(bounds))
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

            result.AddRange(topLeft.Query(bounds));
            result.AddRange(topRight.Query(bounds));
            result.AddRange(bottomRight.Query(bounds));
            result.AddRange(bottomLeft.Query(bounds));

            return result;
        }

        public override bool Insert(MonoObject monoObject)
        {
            if (!monoObject.Bounds.Intersects(boundary))
                return false;

            if (insertionIndex < capacity)
            {
                objects[insertionIndex++] = monoObject;
                return true;
            }
            else
            {
                if (!divided)
                    Subdivide();

                if (topLeft.Insert(monoObject) || topRight.Insert(monoObject) || bottomRight.Insert(monoObject) || bottomLeft.Insert(monoObject))
                    return true;
            }

            return false;
        }

        public override void Clear()
        {
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
            topLeft = new Quadtree(new Rectangle(boundary.X, boundary.Y, boundary.Width / 2, boundary.Height / 2), capacity);
            topRight = new Quadtree(new Rectangle(boundary.X + boundary.Width / 2, boundary.Y, boundary.Width / 2, boundary.Height / 2), capacity);
            bottomRight = new Quadtree(new Rectangle(boundary.X + boundary.Width / 2, boundary.Y + boundary.Height / 2, boundary.Width / 2, boundary.Height / 2), capacity);
            bottomLeft = new Quadtree(new Rectangle(boundary.X, boundary.Y + boundary.Height / 2, boundary.Width / 2, boundary.Height / 2), capacity);
            divided = true;
        }
    }
}


