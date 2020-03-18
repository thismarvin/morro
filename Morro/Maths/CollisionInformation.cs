using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths
{
    class CollisionInformation
    {
        public Vector2[] Vertices { get; private set; }
        public LineSegment[] LineSegments { get; private set; }

        public CollisionInformation(Vector2[] vertices, LineSegment[] lineSegments)
        {
            Vertices = vertices;
            LineSegments = lineSegments;
        }
    }
}
