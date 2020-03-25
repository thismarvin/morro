using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths.Collision
{
    static class SAT
    {
        public static CollisionInformation CalculateCollisionInformation(IShape a, IShape b)
        {
            LineSegment[] aLineSegments = a.LineSegments;

            Vector2[] aVertices = a.Vertices;
            Vector2[] bVertices = b.Vertices;

            LineSegment edge = new LineSegment();
            float minOverlap = float.MaxValue;
            int edgeIndex = -1;
            float overlap;

            Vector2 normal;
            float projection;
            float minProjectionA;
            float maxProjectionA;
            float minProjectionB;
            float maxProjectionB;

            for (int i = 0; i < aLineSegments.Length; i++)
            {
                normal = new Vector2
                (
                    -(aLineSegments[i].Y2 - aLineSegments[i].Y1),
                      aLineSegments[i].X2 - aLineSegments[i].X1
                );

                minProjectionA = Vector2.Dot(aVertices[0], normal);
                maxProjectionA = minProjectionA;

                for (int j = 0; j < aVertices.Length; j++)
                {
                    projection = Vector2.Dot(aVertices[j], normal);
                    minProjectionA = Math.Min(minProjectionA, projection);
                    maxProjectionA = Math.Max(maxProjectionA, projection);
                }

                minProjectionB = Vector2.Dot(bVertices[0], normal);
                maxProjectionB = minProjectionB;

                for (int j = 0; j < bVertices.Length; j++)
                {
                    projection = Vector2.Dot(bVertices[j], normal);
                    minProjectionB = Math.Min(minProjectionB, projection);
                    maxProjectionB = Math.Max(maxProjectionB, projection);
                }

                overlap = Math.Min(maxProjectionA, maxProjectionB) - Math.Max(minProjectionA, minProjectionB);

                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    edge = aLineSegments[i];
                    edgeIndex = i;
                }

                if (!(maxProjectionB >= minProjectionA && maxProjectionA >= minProjectionB))
                    return null;
            }

            return new CollisionInformation(edge, minOverlap, edgeIndex);
        }
    }
}
