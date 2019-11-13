using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    class Quadtree
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

        public bool Insert(MonoObject monoObject)
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

        public List<MonoObject> Query(Rectangle area)
        {
            List<MonoObject> result = new List<MonoObject>();

            //if (!area.Intersects(boundary))
            //    return result;

            if (boundary.EntirelyWithin(area))
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

                    if (area.Intersects(objects[i].Bounds))
                    {
                        result.Add(objects[i]);
                    }
                }
            }

            if (!divided)
                return result;

            result.AddRange(topLeft.Query(area));
            result.AddRange(topRight.Query(area));
            result.AddRange(bottomRight.Query(area));
            result.AddRange(bottomLeft.Query(area));

            return result;
        }

        public List<MonoObject> Query(MonoObject monoObject)
        {
            return Query(monoObject.Bounds);
        }

        public void Clear()
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


