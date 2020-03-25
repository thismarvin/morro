using Microsoft.Xna.Framework;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths
{
    class ShapeScheme
    {
        public Vector2[] Vertices { get; private set; }
        public LineSegment[] LineSegments { get; private set; }

        public ShapeScheme(Vector2[] vertices, LineSegment[] lineSegments)
        {
            if (vertices.Length != lineSegments.Length)
                throw new MorroException("A shape scheme should have the same amount of vertices as line segments.", new ArgumentException());

            Vertices = vertices;
            LineSegments = lineSegments;
        }        
    }
}
