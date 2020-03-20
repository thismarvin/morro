﻿using Microsoft.Xna.Framework;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    static class PolygonHelper
    {
        #region Collision Handling
        public static CollisionInformation GetCollisionInformation(this Polygon polygon)
        {
            Vector2[] transformedVertices = CalculateTransformedVertices(polygon);
            LineSegment[] transformedLineSegments = CalculateTransformedLineSegments(polygon, transformedVertices);

            return new CollisionInformation(transformedVertices, transformedLineSegments);
        }

        private static Vector2[] CalculateTransformedVertices(Polygon polygon)
        {
            Vector2[] result = new Vector2[polygon.ShapeData.TotalVertices];

            for (int i = 0; i < polygon.ShapeData.TotalVertices; i++)
            {
                result[i] = Vector2.Transform(new Vector2(polygon.ShapeData.Vertices[i].X, polygon.ShapeData.Vertices[i].Y), polygon.Transform);
            }

            return result;
        }

        private static LineSegment[] CalculateTransformedLineSegments(Polygon polygon, Vector2[] transformedVertices)
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
        #endregion
    }
}
