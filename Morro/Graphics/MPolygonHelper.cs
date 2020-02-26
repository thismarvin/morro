using Microsoft.Xna.Framework;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    static class MPolygonHelper
    {
        #region Collision Handling
        public static CollisionInformation GetCollisionInformation(this MPolygon polygon)
        {
            Vector2[] transformedVertices = CalculateTransformedVertices(polygon);
            LineSegment[] transformedLineSegments = CalculateTransformedLineSegments(polygon, transformedVertices);

            return new CollisionInformation(transformedVertices, transformedLineSegments);
        }

        private static Vector2[] CalculateTransformedVertices(MPolygon polygon)
        {
            Vector2[] result = new Vector2[polygon.ShapeData.TotalVertices];

            for (int i = 0; i < polygon.ShapeData.TotalVertices; i++)
            {
                result[i] = Vector2.Transform(new Vector2(polygon.ShapeData.Vertices[i].X, polygon.ShapeData.Vertices[i].Y), polygon.Transform);
            }

            return result;
        }

        private static LineSegment[] CalculateTransformedLineSegments(MPolygon polygon, Vector2[] transformedVertices)
        {
            if (!polygon.Filled)
                return CalculateHollowLineSegments(polygon, transformedVertices);

            return CalculateFilledLineSegments(polygon, transformedVertices);
        }

        private static LineSegment[] CalculateFilledLineSegments(MPolygon polygon, Vector2[] transformedVertices)
        {
            int totalVertices = polygon.ShapeData.TotalVertices;
            LineSegment[] result = new LineSegment[totalVertices];

            result[0] = new LineSegment(transformedVertices[totalVertices - 1].X, transformedVertices[totalVertices - 1].Y, transformedVertices[0].X, transformedVertices[0].Y);

            for (int i = 1; i < totalVertices; i++)
            {
                result[i] = new LineSegment(transformedVertices[i - 1].X, transformedVertices[i - 1].Y, transformedVertices[i].X, transformedVertices[i].Y);
            }

            return result;
        }

        private static LineSegment[] CalculateHollowLineSegments(MPolygon polygon, Vector2[] transformedVertices)
        {
            int totalVertices = polygon.ShapeData.TotalVertices / 2;
            LineSegment[] result = new LineSegment[totalVertices];

            result[0] = new LineSegment(transformedVertices[totalVertices - 1].X, transformedVertices[totalVertices - 1].Y, transformedVertices[totalVertices].X, transformedVertices[totalVertices].Y);

            for (int i = 1; i < totalVertices; i++)
            {
                result[i] = new LineSegment(transformedVertices[(totalVertices + i) - 1].X, transformedVertices[(totalVertices + i) - 1].Y, transformedVertices[totalVertices + i].X, transformedVertices[totalVertices + i].Y);
            }

            return result;
        }
        #endregion
    }
}
