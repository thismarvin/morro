using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    struct MTV
    {
        public LineSegment Edge { get; private set; }
        public int EdgeIndex { get; private set; }
        public float Overlap { get; private set; }
        
        public MTV(LineSegment edge, float overlap, int edgeIndex)
        {
            Edge = edge;
            Overlap = overlap;
            EdgeIndex = edgeIndex;
        }
    }
}
