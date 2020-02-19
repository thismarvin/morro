using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    class Bin : Partitioner
    {
        private HashSet<PartitionEntry>[] buckets;
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
            columns = (int)Math.Ceiling((float)Boundary.Width / powerOfTwo);
            rows = (int)Math.Ceiling((float)Boundary.Height / powerOfTwo);

            buckets = new HashSet<PartitionEntry>[rows * columns];

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new HashSet<PartitionEntry>();
            }
        }

        public override List<PartitionEntry> Query(Rectangle bounds)
        {
            List<PartitionEntry> result = new List<PartitionEntry>();
            HashSet<PartitionEntry> entries = new HashSet<PartitionEntry>();
            HashSet<int> ids = HashIDs(bounds);

            foreach (int id in ids)
            {
                foreach (PartitionEntry entry in buckets[id])
                {
                    entries.Add(entry);
                }
            }

            foreach (PartitionEntry entry in entries)
            {
                result.Add(entry);
            }

            entries.Clear();
            ids.Clear();

            return result;
        }

        public override bool Insert(PartitionEntry entry)
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

