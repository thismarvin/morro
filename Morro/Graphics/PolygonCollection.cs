using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class PolygonCollection : DrawCollection<MPolygon>
    {
        public PolygonCollection() : base(100000)
        {
        }

        protected override DrawGroup<MPolygon> CreateDrawGroup(MPolygon currentEntry, int capacity)
        {
            return new PolygonGroup(currentEntry.ShapeData, capacity);
        }
    }
}
