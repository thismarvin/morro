using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Morro.Utilities
{
    class SparseSet : IEnumerable<uint>
    {
        public int Count { get { return (int)n; } }

        private readonly uint[] dense;
        private readonly uint[] sparse;
        /// <summary>
        /// The maximum amount of elements allowed inside the sparse set.
        /// </summary>
        private readonly uint u;
        /// <summary>
        /// The current index of the sparse set.
        /// </summary>
        private uint n;

        /// <summary>
        /// A data structure that stores a set of <see cref="uint"/>. 
        /// </summary>
        /// <param name="size">The maximum amount of elements allowed inside the sparse set.</param>
        public SparseSet(int size)
        {
            u = (uint)size;
            dense = new uint[u];
            sparse = new uint[u];
        }

        public void Add(uint k)
        {
            if (!(0 <= k && k < u) || Contains(k))
                return;

            dense[n] = k;
            sparse[k] = n;
            n++;
        }

        public void Remove(uint k)
        {
            if (!Contains(k))
                return;

            n--;

            for (int i = Array.IndexOf(dense, k); i < n; i++)
            {
                dense[i] = dense[i + 1];
                sparse[dense[i + 1]] = (uint)i;
            }
        }

        public bool Contains(uint k)
        {
            return k < u && sparse[k] < n && dense[sparse[k]] == k;
        }

        public void Clear()
        {
            n = 0;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("{ ");
            for (int i = 0; i < n; i++)
            {
                stringBuilder.Append(dense[i]);

                if (i != n - 1)
                {
                    stringBuilder.Append(", ");
                }
            }
            stringBuilder.Append(" }");

            return stringBuilder.ToString();
        }

        public IEnumerator<uint> GetEnumerator()
        {
            return new SparseSetEnumerator(dense, n);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SparseSetEnumerator(dense, n);
        }

        internal sealed class SparseSetEnumerator : IEnumerator<uint>
        {
            private readonly uint[] dense;
            private readonly uint n;

            private int index;

            public uint Current { get { return dense[index]; } }

            object IEnumerator.Current { get { return dense[index]; } }

            public SparseSetEnumerator(uint[] dense, uint n)
            {
                this.dense = dense;
                this.n = n;
                index = -1;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                index++;
                return index < n;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }
}
