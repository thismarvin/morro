﻿using Microsoft.Xna.Framework.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    abstract class DrawGroup<T>
    {
        protected readonly int capacity;
        protected T[] group;
        protected int groupIndex;

        public DrawGroup(int capacity)
        {
            this.capacity = capacity;
            group = new T[this.capacity];
        }

        public void Clear()
        {
            Array.Clear(group, 0, group.Length);
        }

        public virtual bool Add(T entry)
        {
            if (groupIndex >= capacity)
                return false;

            if (ConditionToAdd(entry))
            {
                group[groupIndex++] = entry;
                return true;
            }

            return false;
        }

        protected abstract bool ConditionToAdd(T entry);
        public abstract void Draw(Camera camera);
    }
}
