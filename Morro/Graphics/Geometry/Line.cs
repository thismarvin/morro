using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Graphics
{
    class Line : Polygon
    {
        protected List<Vector2> points;
        protected List<Vector2> scaledPoints;
        protected List<Vector2> linePoints;
        protected List<Vector2> intersections;
        protected int totalPoints;

        public Vector2 StartingPoint { get { return points[0]; } }
        public Vector2 EndPoint { get { return points[points.Count - 1]; } }

        public Line(float x1, float y1, float x2, float y2, float lineWidth, Color color, VertexInformation vertexInformation) : base(0, 0, 1, 1, 4, lineWidth, color, vertexInformation)
        {
            points = new List<Vector2>()
            {
                new Vector2(x1,y1),
                new Vector2(x2,y2),
            };
            totalPoints = points.Count;

            ScaleDownPoints();
            CreatePolygon();
        }

        public Line(List<Vector2> points, float lineWidth, Color color, VertexInformation vertexInformation) : base(0, 0, 1, 1, points.Count * 2, lineWidth, color, vertexInformation)
        {
            if (points.Count < 2)
            {
                throw new Exception("A line must have at least two points.");
            }

            this.points = points;
            totalPoints = points.Count;

            ScaleDownPoints();
            CreatePolygon();
        }

        public override void SetLocation(float x, float y)
        {
            SetStartingPoint(x, y);
        }

        public void SetStartingPoint(float x, float y)
        {
            points[0] = new Vector2(x, y);
            ScaleDownPoints();
            CreateVertices();
        }

        public void SetEndPoint(float x, float y)
        {
            points[points.Count - 1] = new Vector2(x, y);
            ScaleDownPoints();
            CreateVertices();
        }

        protected override void SetupPolygonInformation()
        {
            Filled = true;
            totalVertices = initialTotalVertices;
            totalTriangles = totalVertices - 2;
            totalIndices = totalTriangles * 3;
        }

        protected override void CreateKey()
        {
            vertexKey = string.Format(CultureInfo.InvariantCulture, "L-{0}V-{1}LW-C{2}", initialTotalVertices, LineWidth, Color);
            indexKey = string.Format(CultureInfo.InvariantCulture, "L-{0}V", initialTotalVertices);
        }

        protected override void CreateIndices()
        {
            indices = new short[totalIndices];

            int i = 0;
            int j = 0;
            for (; i < totalPoints - 1; i++)
            {
                indices[j] = (short)(i);
                indices[j + 1] = (short)(totalPoints * 2 - 1 - i - 1);
                indices[j + 2] = (short)(i + 1);
                j += 3;
            }

            i = 0;
            for (; i < totalPoints - 1; i++)
            {
                indices[j] = (short)(totalPoints * 2 - 1 - i);
                indices[j + 1] = (short)(totalPoints * 2 - 1 - i - 1);
                indices[j + 2] = (short)(i);
                j += 3;
            }
        }

        protected override void CreateVertices()
        {
            if (scaledPoints.Count == 0)
                return;

            vertices = new VertexPositionColor[totalVertices];

            CreateSloppyVertices();
            FindBetterVertices();
            ConnectVertices();
        }

        protected override void CreateTransformedLineSegments()
        {

        }

        private float FindMinimumValue(List<Vector2> vectors, int vertexAxisIndex)
        {
            float min = vertexAxisIndex == 0 ? vectors[0].X : vectors[0].Y;

            foreach (Vector2 v in vectors)
            {
                if (vertexAxisIndex == 0)
                {
                    min = v.X < min ? v.X : min;
                }
                else if (vertexAxisIndex == 1)
                {
                    min = v.Y < min ? v.Y : min;
                }
            }
            return min;
        }

        private float FindMaximumValue(List<Vector2> vectors, int vertexAxisIndex)
        {
            float max = vertexAxisIndex == 0 ? vectors[0].X : vectors[0].Y;

            foreach (Vector2 v in vectors)
            {
                if (vertexAxisIndex == 0)
                {
                    max = v.X > max ? v.X : max;
                }
                else if (vertexAxisIndex == 1)
                {
                    max = v.Y > max ? v.Y : max;
                }
            }
            return max;
        }

        private void ScaleDownPoints()
        {
            // Initialize lists.
            scaledPoints = new List<Vector2>();
            List<Vector2> pointsCopy = new List<Vector2>();

            // Create a copy of points.
            foreach (Vector2 v in points)
            {
                pointsCopy.Add(new Vector2(v.X, v.Y));
            }

            // Find the minimums and maximums of each axis.
            float minX = FindMinimumValue(pointsCopy, 0);
            float minY = FindMinimumValue(pointsCopy, 1);
            float maxX = FindMaximumValue(pointsCopy, 0);
            float maxY = FindMaximumValue(pointsCopy, 1);

            // Find the range of each axis.
            float xRange = maxX - minX;
            float yRange = maxY - minY;

            xRange = xRange == 0 ? LineWidth : xRange;
            yRange = yRange == 0 ? LineWidth : yRange;

            // Set the top left of the line's bounds to the minimum values of each axis, and set the dimensions of the line's bounds to the axis's range.
            SetBounds(minX, minY, (int)Math.Ceiling(xRange), (int)Math.Ceiling(yRange));

            // Flip the Y values of the points in the copied list of points.
            for (int i = 0; i < totalPoints; i++)
            {
                pointsCopy[i] = new Vector2(pointsCopy[i].X, -pointsCopy[i].Y);
            }

            // Find the new minimum and maximum values of the Y axis.
            minY = FindMinimumValue(pointsCopy, 1);
            maxY = FindMaximumValue(pointsCopy, 1);

            // Translate the points out of the negative axis. The points should all be in the first quadrant.
            if (minX < 0 || minY < 0)
            {
                for (int i = 0; i < totalPoints; i++)
                {
                    pointsCopy[i] = new Vector2(pointsCopy[i].X + (minX < 0 ? -minX : 0), pointsCopy[i].Y + (minY < 0 ? -minY : 0));
                }

                if (minX < 0)
                {
                    minX = FindMinimumValue(pointsCopy, 0);
                    maxX = FindMaximumValue(pointsCopy, 0);
                }
                if (minY < 0)
                {
                    minY = FindMinimumValue(pointsCopy, 1);
                    maxY = FindMaximumValue(pointsCopy, 1);
                }
            }

            // Find the new range of each axis.
            xRange = maxX - minX;
            yRange = maxY - minY;        

            xRange = xRange == 0 ? 1 : xRange;
            yRange = yRange == 0 ? 1 : yRange;

            // Scale down the points to fit in a 1x1 square. This is required for initial vertex calculations.
            for (int i = 0; i < totalPoints; i++)
            {
                scaledPoints.Add(new Vector2(-(pointsCopy[i].X - minX) / xRange, (pointsCopy[i].Y - minY) / yRange));
            }
        }

        private void CreateSloppyVertices()
        {
            linePoints = new List<Vector2>();
            float theta;
            float scaledLineWidthX = LineWidth / Width;
            float scaledLineWidthY = LineWidth / Height;

            for (int i = 1; i < totalPoints; i++)
            {
                theta = (float)(-MathHelper.PiOver2 + Math.Atan2(scaledPoints[i].Y * Height - scaledPoints[i - 1].Y * Height, scaledPoints[i].X * Width - scaledPoints[i - 1].X * Width));

                linePoints.Add(new Vector2((float)(scaledPoints[i - 1].X - Math.Cos(theta) * (scaledLineWidthX / 2)), (float)(scaledPoints[i - 1].Y - Math.Sin(theta) * (scaledLineWidthY / 2))));
                linePoints.Add(new Vector2((float)(scaledPoints[i].X - Math.Cos(theta) * (scaledLineWidthX / 2)), (float)(scaledPoints[i].Y - Math.Sin(theta) * (scaledLineWidthY / 2))));
            }

            for (int i = totalPoints - 1; i >= 1; i--)
            {
                theta = (float)(-MathHelper.PiOver2 + Math.Atan2(scaledPoints[i].Y * Height - scaledPoints[i - 1].Y * Height, scaledPoints[i].X * Width - scaledPoints[i - 1].X * Width));

                linePoints.Add(new Vector2((float)(scaledPoints[i].X + Math.Cos(theta) * (scaledLineWidthX / 2)), (float)(scaledPoints[i].Y + Math.Sin(theta) * (scaledLineWidthY / 2))));
                linePoints.Add(new Vector2((float)(scaledPoints[i - 1].X + Math.Cos(theta) * (scaledLineWidthX / 2)), (float)(scaledPoints[i - 1].Y + Math.Sin(theta) * (scaledLineWidthY / 2))));
            }
        }

        private void FindBetterVertices()
        {
            intersections = new List<Vector2>();

            for (int i = 0; i < totalPoints - 2; i++)
            {
                intersections.Add(PointOfIntersection(linePoints[(i * 2)], linePoints[(i * 2) + 1], linePoints[(i * 2) + 2], linePoints[(i * 2) + 3]));
            }

            for (int i = 0; i < totalPoints - 2; i++)
            {
                intersections.Add(PointOfIntersection(linePoints[(totalPoints - 1) * 2 + i * 2], linePoints[(totalPoints - 1) * 2 + i * 2 + 1], linePoints[(totalPoints - 1) * 2 + i * 2 + 2], linePoints[(totalPoints - 1) * 2 + i * 2 + 3]));
            }
        }

        private void ConnectVertices()
        {
            int vertexIndex = 0;
            int intersectionIndex = 0;

            for (int i = 0; i < totalPoints; i++)
            {
                if (i == 0)
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(linePoints[0].X, linePoints[0].Y, 0), Color);
                else if (i == totalPoints - 1)
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(linePoints[2 * totalPoints - 3].X, linePoints[2 * totalPoints - 3].Y, 0), Color);
                else
                {
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(intersections[intersectionIndex].X, intersections[intersectionIndex].Y, 0), Color);
                    intersectionIndex++;
                }
            }

            for (int i = 0; i < totalPoints; i++)
            {
                if (i == 0)
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(linePoints[2 * totalPoints - 3 + 1].X, linePoints[2 * totalPoints - 3 + 1].Y, 0), Color);
                else if (i == totalPoints - 1)
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(linePoints[linePoints.Count - 1].X, linePoints[linePoints.Count - 1].Y, 0), Color);
                else
                {
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(intersections[intersectionIndex].X, intersections[intersectionIndex].Y, 0), Color);
                    intersectionIndex++;
                }
            }
        }

        private Vector2 PointOfIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            // Prevents a slope of NaN.
            if (a2.X == a1.X)
            {
                a2.X += 0.0001f;
            }
            if (b2.X == b1.X)
            {
                b2.X += 0.0001f;
            }

            float aSlope = (a2.Y - a1.Y) / (a2.X - a1.X);
            float aYIntercept = a1.Y - aSlope * a1.X;

            float bSlope = (b2.Y - b1.Y) / (b2.X - b1.X);
            float bYIntercept = b1.Y - bSlope * b1.X;

            // Handles parallel lines.
            if (Math.Round(aSlope - bSlope, 3) == 0)
                return a2;

            float x = (bYIntercept - aYIntercept) / (aSlope - bSlope);
            float y = aSlope * x + aYIntercept;

            return new Vector2(x, y);
        }

        protected override void CreateTransform()
        {
            transform =
                // Scale unit vertices by the lines's dimensions.
                Matrix.CreateScale(Width, Height, 1) *

                // Translate the origin of line down by its height. It's too high becasaue the Y values were flipped!
                Matrix.CreateTranslation(0, -Height, 0) *

                // Translate to the polygon's axis of rotaion, rotate the polygon, and then translate back to the top left.
                Matrix.CreateTranslation(rotationOffset.X, rotationOffset.Y, 0) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(-rotationOffset.X, -rotationOffset.Y, 0) *

                // Translate the polygon to the inverse of its position.
                Matrix.CreateTranslation(-X, -Y, 0) *
                Matrix.Identity;
        }

        protected override void CreateFilledIndices()
        {

        }

        protected override void CreateHollowIndices()
        {

        }

        protected override void CreateFilledVertices()
        {

        }

        protected override void CreateHollowVertices()
        {

        }
    }
}
