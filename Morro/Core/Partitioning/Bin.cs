using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    class Bin : Partitioner
    {
        private readonly HashSet<MonoObject>[] buckets;
        private readonly Rectangle boundary;
        private readonly int powerOfTwo;
        private readonly int columns;
        private readonly int rows;

        public Bin(Rectangle boundary, int powerOfTwo)
        {
            this.boundary = boundary;
            this.powerOfTwo = powerOfTwo;

            columns = (int)Math.Ceiling((float)boundary.Width / powerOfTwo);
            rows = (int)Math.Ceiling((float)boundary.Height / powerOfTwo);

            buckets = new HashSet<MonoObject>[rows * columns];

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new HashSet<MonoObject>();
            }
        }

        public override List<MonoObject> Query(Rectangle bounds)
        {
            List<MonoObject> result = new List<MonoObject>();
            HashSet<MonoObject> objects = new HashSet<MonoObject>();
            HashSet<int> ids = HashIDs(bounds);

            foreach (int id in ids)
            {
                foreach (MonoObject monoObject in buckets[id])
                {
                    objects.Add(monoObject);
                }
            }

            foreach (MonoObject monoObject in objects)
            {
                result.Add(monoObject);
            }

            objects.Clear();
            ids.Clear();

            return result;
        }

        public override bool Insert(MonoObject monoObject)
        {
            if (!monoObject.Bounds.Intersects(boundary))
                return false;

            HashSet<int> ids = HashIDs(monoObject.Bounds);

            foreach (int i in ids)
            {
                buckets[i].Add(monoObject);
            }

            return ids.Count > 0;
        }

        public override void Clear()
        {
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i].Clear();
            }
        }

        private HashSet<int> HashIDs(Rectangle bounds)
        {
            HashSet<int> result = new HashSet<int>();
            int x = -1;
            int y = -1;
            int cellSize = 1 << powerOfTwo;

            if (bounds.Width > cellSize || bounds.Height > cellSize)
            {
                // YIKES
                for (int heightOffset = 0; heightOffset < bounds.Height; heightOffset += cellSize)
                {
                    for (int widthOffset = 0; widthOffset < bounds.Width; widthOffset += cellSize)
                    {
                        x = (int)((bounds.X + widthOffset)) >> powerOfTwo;
                        y = (int)((bounds.Y + heightOffset)) >> powerOfTwo;

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
                        x = (int)((bounds.Left)) >> powerOfTwo;
                        y = (int)((bounds.Top)) >> powerOfTwo;
                        break;
                    case 1:
                        x = (int)((bounds.Right)) >> powerOfTwo;
                        y = (int)((bounds.Top)) >> powerOfTwo;
                        break;
                    case 2:
                        x = (int)((bounds.Right)) >> powerOfTwo;
                        y = (int)((bounds.Bottom)) >> powerOfTwo;
                        break;
                    case 3:
                        x = (int)((bounds.Left)) >> powerOfTwo;
                        y = (int)((bounds.Bottom)) >> powerOfTwo;
                        break;
                }

                if (x < 0 || x >= columns || y < 0 || y >= rows)
                    continue;

                result.Add(columns * y + x);
            }

            return result;
        }
    }
}

