using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class Bin<T> : Partitioner<T> where T : IPartitionable
    {
        private HashSet<T>[] buckets;
        private readonly int powerOfTwo;
        private int columns;
        private int rows;

        public Bin(Rectangle boundary, int powerOfTwo) : base(boundary)
        {
            this.powerOfTwo = powerOfTwo;

            Initialize();
        }

        protected override void Initialize()
        {
            columns = (int)Math.Ceiling(Boundary.Width / Math.Pow(2, powerOfTwo));
            rows = (int)Math.Ceiling(Boundary.Height / Math.Pow(2, powerOfTwo));

            buckets = new HashSet<T>[rows * columns];

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new HashSet<T>();
            }
        }

        public override HashSet<T> Query(Rectangle bounds)
        {
            HashSet<T> result = new HashSet<T>();
            HashSet<int> ids = HashIDs(bounds);
            
            foreach (int id in ids)
            {
                foreach (T entry in buckets[id])
                {
                    if (entry == null)
                        continue;

                    result.Add(entry);
                }
            }

            return result;
        }

        // TODO: The freeze error is because of this. Maybe have like a buffer that adds all the entries once this is complete?
        public override bool Insert(T entry)
        {
            if (!entry.Bounds.Intersects(Boundary))
                return false;

            HashSet<int> ids = HashIDs(entry.Bounds);

            foreach (int i in ids)
            {
                buckets[i].Add(entry);
            }

            return ids.Count > 0;
        }

        public override void Clear()
        {
            if (buckets == null)
                return;

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i].Clear();
            }
        }

        private HashSet<int> HashIDs(Rectangle bounds)
        {
            Rectangle validatedBounds = ValidateBounds();
            HashSet<int> result = new HashSet<int>();
            int x = -1;
            int y = -1;
            int cellSize = 1 << powerOfTwo;

            if (validatedBounds.Width > cellSize || validatedBounds.Height > cellSize)
            {
                // YIKES
                for (int heightOffset = 0; heightOffset < validatedBounds.Height; heightOffset += cellSize)
                {
                    for (int widthOffset = 0; widthOffset < validatedBounds.Width; widthOffset += cellSize)
                    {
                        x = (int)(validatedBounds.X + widthOffset) >> powerOfTwo;
                        y = (int)(validatedBounds.Y + heightOffset) >> powerOfTwo;

                        if (x < 0 || x >= columns || y < 0 || y >= rows)
                            continue;

                        result.Add(columns * y + x);
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        x = (int)validatedBounds.Left >> powerOfTwo;
                        y = (int)validatedBounds.Top >> powerOfTwo;
                        break;
                    case 1:
                        x = (int)validatedBounds.Right >> powerOfTwo;
                        y = (int)validatedBounds.Top >> powerOfTwo;
                        break;
                    case 2:
                        x = (int)validatedBounds.Right >> powerOfTwo;
                        y = (int)validatedBounds.Bottom >> powerOfTwo;
                        break;
                    case 3:
                        x = (int)validatedBounds.Left >> powerOfTwo;
                        y = (int)validatedBounds.Bottom >> powerOfTwo;
                        break;
                }

                if (x < 0 || x >= columns || y < 0 || y >= rows)
                    continue;

                result.Add(columns * y + x);
            }

            return result;

            Rectangle ValidateBounds()
            {
                int _x = (int)bounds.X;
                int _y = (int)bounds.Y;
                int _width = (int)Math.Ceiling(bounds.Width);
                int _height = (int)Math.Ceiling(bounds.Height);

                _x = Math.Max(0, _x);
                _x = Math.Min((int)Boundary.Right, _x);

                _y = Math.Max(0, _y);
                _y = Math.Min((int)Boundary.Bottom, _y);

                _width = Math.Min((int)Math.Ceiling(Boundary.Right) - _x, _width);
                _height = Math.Min((int)Math.Ceiling(Boundary.Bottom) - _y, _height);

                return new Rectangle(_x, _y, _width, _height);
            }
        }
    }
}
