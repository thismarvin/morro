﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public enum ShapeType
    {
        Triangle,
        RightTriangle,
        Square,
        Star,
        Circle
    }

    static class GeometryManager
    {
        public static Effect PolygonShader { get; private set; }

        private static readonly ResourceHandler<ShapeData> shapes;

        static GeometryManager()
        {
            PolygonShader = AssetManager.GetEffect("PolygonShader").Clone();
            shapes = new ResourceHandler<ShapeData>();

            RegisterTriangle();
            RegisterRightTriangle();
            RegisterSquare();
            RegisterCircle();
            RegisterStar();
        }

        #region Handle Shape Data
        /// <summary>
        /// Register <see cref="ShapeData"/> to be managed by Morro.
        /// </summary>
        /// <param name="name">The name that the shape data being registered will be referenced as.</param>
        /// <param name="shapeData">The shape data you want to be registered.</param>
        public static void RegisterShapeData(string name, ShapeData shapeData)
        {
            shapes.Register(name, shapeData);
        }

        /// <summary>
        /// Get <see cref="ShapeData"/> that was previously registered.
        /// </summary>
        /// <param name="name">The name given to shape data that was already registered.</param>
        /// <returns>The registered shape data with the given name.</returns>
        public static ShapeData GetShapeData(string name)
        {
            return shapes.Get(name);
        }

        /// <summary>
        /// Get <see cref="ShapeData"/> that was registered by Morro.
        /// </summary>
        /// <param name="shapeType">The basic shape data you want to get.</param>
        /// <returns>The registered shape data with the given name.</returns>
        public static ShapeData GetShapeData(ShapeType shapeType)
        {
            return GetShapeData($"Morro_{shapeType.ToString()}");
        }

        /// <summary>
        /// Remove registered <see cref="ShapeData"/>.
        /// </summary>
        /// <param name="name">The name given to shape data that was already registered.</param>
        public static void RemoveShapeData(string name)
        {
            shapes.Remove(name);
        }
        #endregion

        /// <summary>
        /// Sets <see cref="PolygonShader"/>'s "WorldViewProjection" parameter to the camera's <see cref="Camera.WorldViewProjection"/>.
        /// </summary>
        /// <param name="camera">The target camera.</param>
        public static void SetupPolygonShader(Camera camera)
        {
            PolygonShader.Parameters["WorldViewProjection"].SetValue(camera.WorldViewProjection);
        }

        public static ShapeData CreateRegularPolygon(int totalVertices)
        {
            if (totalVertices <= 2)
                throw new MorroException("A polygon must have at least 3 vertices.", new ArgumentException());

            List<Vector3> vertices = new List<Vector3>();
            List<short> indices = new List<short>();
            int totalTriangles = totalVertices - 2;
            int totalIndices = totalTriangles * 3;
            float angleIncrement = MathHelper.TwoPi / totalVertices;

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                vertices.Add(new Vector3(0.5f + (float)Math.Cos(i) * 0.5f, 0.5f + (float)Math.Sin(i) * 0.5f, 0));

                if (vertices.Count >= totalVertices)
                    break;
            }

            int j = 1;
            for (int i = 0; i < totalIndices; i += 3)
            {
                indices.Add(0);
                indices.Add((short)(j + 1));
                indices.Add((short)j);
                j++;
            }

            return new ShapeData(vertices.ToArray(), indices.ToArray());
        }

        public static ShapeData CreateHollowSquare(float width, float height, float lineWidth)
        {
            int initialTotalVertices = 4;
            int totalVertices = initialTotalVertices * 2;
            int totalTriangles = initialTotalVertices * 2;
            int totalIndices = totalTriangles * 3;

            float scaledLineWidthX = lineWidth / width;
            float scaledLineWidthY = lineWidth / height;

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(scaledLineWidthX, scaledLineWidthY, 0),
                new Vector3(scaledLineWidthX, 1 - scaledLineWidthY, 0),
                new Vector3(1 - scaledLineWidthX, 1 - scaledLineWidthY, 0),
                new Vector3(1 - scaledLineWidthX, scaledLineWidthY, 0),

                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 0, 0),
            };

            short[] indices = CreateHollowIndices(totalVertices, totalIndices);

            return new ShapeData(vertices, indices);
        }

        public static ShapeData CreateHollowCircle(float radius, float lineWidth)
        {
            return CreateHollowRegularPolygon(90, radius * 2, radius * 2, lineWidth);
        }

        public static ShapeData CreateHollowRegularPolygon(int totalVertices, float width, float height, float lineWidth)
        {
            if (totalVertices <= 2)
                throw new MorroException("A polygon must have at least 3 vertices.", new ArgumentException());

            int initialTotalVertices = totalVertices;
            int _totalVertices = initialTotalVertices * 2;
            int totalTriangles = initialTotalVertices * 2;
            int totalIndices = totalTriangles * 3;

            Vector3[] vertices = new Vector3[_totalVertices];

            float angleIncrement = MathHelper.TwoPi / totalVertices;

            float theta;
            if (initialTotalVertices == 3)
            {
                theta = (float)Math.PI / 6;
            }
            else
            {
                theta = MathHelper.TwoPi / initialTotalVertices;
                theta = MathHelper.Pi - theta;
                theta /= 2;
            }

            float scaledLineWidthX = (lineWidth / width) / (float)Math.Sin(theta);
            float scaledLineWidthY = (lineWidth / height) / (float)Math.Sin(theta);
            int vertexIndex = 0;

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                vertices[vertexIndex++] = new Vector3(0.5f + (float)Math.Cos(i) * (0.5f - scaledLineWidthX), 0.5f + (float)Math.Sin(i) * (0.5f - scaledLineWidthY), 0);

                if (vertexIndex >= vertices.Length / 2)
                    break;
            }

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                vertices[vertexIndex++] = new Vector3(0.5f + (float)Math.Cos(i) * 0.5f, 0.5f + (float)Math.Sin(i) * 0.5f, 0);

                if (vertexIndex >= vertices.Length)
                    break;
            }

            short[] indices = CreateHollowIndices(_totalVertices, totalIndices);

            return new ShapeData(vertices, indices);
        }

        private static short[] CreateHollowIndices(int totalVertices, int totalIndices)
        {
            short[] indices = new short[totalIndices];
            int i = 0;
            int j = 0;
            for (; i < totalVertices / 2 - 1; i++)
            {
                indices[j] = (short)(i);
                indices[j + 1] = (short)(i + (totalVertices / 2 + 1));
                indices[j + 2] = (short)(i + (totalVertices / 2));
                j += 3;
            }

            indices[j] = (short)(i);
            indices[j + 1] = (short)(totalVertices / 2);
            indices[j + 2] = (short)(i + (totalVertices / 2));
            j += 3;
            i++;

            indices[j] = (short)(i);
            indices[j + 1] = (short)(totalVertices / 2 - 1);
            indices[j + 2] = (short)(i - (totalVertices / 2));
            j += 3;
            i++;

            for (; i < totalVertices; i++)
            {
                indices[j] = (short)(i);
                indices[j + 1] = (short)(i - (totalVertices / 2 + 1));
                indices[j + 2] = (short)(i - (totalVertices / 2));
                j += 3;
            }

            return indices;
        }

        private static void RegisterTriangle()
        {
            ShapeData shapeData = CreateRegularPolygon(3);
            shapeData.Managed = true;
            RegisterShapeData("Morro_Triangle", shapeData);
        }

        private static void RegisterRightTriangle()
        {
            ShapeData rightTriangleData = new ShapeData
            (
                new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 1, 0),
                },
                new short[]
                {
                    0, 1, 2,
                },
                true
            );

            RegisterShapeData("Morro_RightTriangle", rightTriangleData);
        }

        private static void RegisterSquare()
        {
            ShapeData squareData = new ShapeData
            (
                new Vector3[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 1, 0),
                },
                new short[]
                {
                    0, 1, 2,
                    0, 2, 3
                },
                true
            );

            RegisterShapeData("Morro_Square", squareData);
        }

        private static void RegisterCircle()
        {
            ShapeData shapeData = CreateRegularPolygon(90);
            shapeData.Managed = true;
            RegisterShapeData("Morro_Circle", shapeData);
        }

        private static void RegisterStar()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<short> indices = new List<short>();
            int totalVertices = 10;
            int totalTriangles = totalVertices - 2;
            int totalIndices = totalTriangles * 3;
            float angleIncrement = MathHelper.TwoPi / totalVertices;
            int alternate = 0;

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                if (alternate++ % 2 == 0)
                {
                    vertices.Add(new Vector3(0.5f + (float)Math.Cos(i) * 0.25f, 0.5f + (float)Math.Sin(i) * 0.25f, 0));
                }
                else
                {
                    vertices.Add(new Vector3(0.5f + (float)Math.Cos(i) * 0.5f, 0.5f + (float)Math.Sin(i) * 0.5f, 0));
                }

                if (vertices.Count >= totalVertices)
                    break;
            }

            int j = 1;
            for (int i = 0; i < totalIndices; i += 3)
            {
                indices.Add(0);
                indices.Add((short)(j + 1));
                indices.Add((short)j);
                j++;
            }

            RegisterShapeData("Morro_Star", new ShapeData(vertices.ToArray(), indices.ToArray(), true));
        }
    }
}
