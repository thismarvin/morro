using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths.Collision
{
    class CollisionInformation
    {
        public LineSegment Edge { get; private set; }
        public int EdgeIndex { get; private set; }
        public float Overlap { get; private set; }

        public CollisionInformation(LineSegment edge, float overlap, int edgeIndex)
        {
            Edge = edge;
            Overlap = overlap;
            EdgeIndex = edgeIndex;
        }
    }
}
