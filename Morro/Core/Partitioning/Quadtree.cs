﻿using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    class Quadtree : Partitioner
    {
        private readonly int capacity;
        private readonly int entityCapacity;
        private bool divided;
        private int insertionIndex;
        private MorroObject[] objects;
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
            objects = new MorroObject[capacity];
        }

        public override HashSet<MorroObject> Query(Rectangle bounds)
        {
            HashSet<MorroObject> result = new HashSet<MorroObject>();

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

            AccumulateObjects(topLeft.Query(bounds));
            AccumulateObjects(topRight.Query(bounds));
            AccumulateObjects(bottomRight.Query(bounds));
            AccumulateObjects(bottomLeft.Query(bounds));

            return result;

            void AccumulateObjects(HashSet<MorroObject> morroObjects)
            {
                foreach (MorroObject morroObject in morroObjects)
                {
                    result.Add(morroObject);
                }
            }
        }

        public override bool Insert(MorroObject morroObject)
        {
            if (!morroObject.Bounds.Intersects(Boundary))
                return false;

            if (insertionIndex < capacity)
            {
                objects[insertionIndex++] = morroObject;
                return true;
            }
            else
            {
                if (!divided)
                    Subdivide();

                if (topLeft.Insert(morroObject) || topRight.Insert(morroObject) || bottomRight.Insert(morroObject) || bottomLeft.Insert(morroObject))
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
    }
}