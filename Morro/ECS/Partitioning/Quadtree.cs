﻿using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    /// <summary>
    /// An implementation of a <see cref="Partitioner{T}"/> that uses a recursive tree structure to store and retrieve <see cref="IPartitionable"/> items.
    /// </summary>
    /// <typeparam name="T">The element stored within the partitioner.</typeparam>
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

        /// <summary>
        /// Creates an implementation of a <see cref="Partitioner{T}"/> that uses a recursive tree structure to store and retrieve <see cref="IPartitionable"/> items.
        /// </summary>
        /// <param name="boundary">The area that the partitioner will cover.</param>
        /// <param name="capacity">The total amount of entities that exist in a node before overflowing into a new tree.</param>
        public Quadtree(RectangleF boundary, int capacity) : base(boundary)
        {
            this.capacity = capacity;

            Initialize();
        }

        protected override void Initialize()
        {
            objects = new T[capacity];
        }

        public override List<int> Query(RectangleF bounds)
        {
            List<int> result = new List<int>();

            if (Boundary.EntirelyWithin(bounds))
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] == null)
                        continue;

                    result.Add(objects[i].Identifier);
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
                        result.Add(objects[i].Identifier);
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
            topLeft = new Quadtree<T>(new RectangleF(Boundary.X, Boundary.Y, Boundary.Width / 2, Boundary.Height / 2), capacity);
            topRight = new Quadtree<T>(new RectangleF(Boundary.X + Boundary.Width / 2, Boundary.Y, Boundary.Width / 2, Boundary.Height / 2), capacity);
            bottomRight = new Quadtree<T>(new RectangleF(Boundary.X + Boundary.Width / 2, Boundary.Y + Boundary.Height / 2, Boundary.Width / 2, Boundary.Height / 2), capacity);
            bottomLeft = new Quadtree<T>(new RectangleF(Boundary.X, Boundary.Y + Boundary.Height / 2, Boundary.Width / 2, Boundary.Height / 2), capacity);
            divided = true;
        }
    }
}
