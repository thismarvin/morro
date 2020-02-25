using Microsoft.Xna.Framework;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    public enum ShapeType
    {
        Triangle,
        RightTriangle,
        Square,
        Star,
        Circle
    }

    static class Geometry
    {
        private static readonly ResourceHandler<ShapeData> shapes;

        static Geometry()
        {
            shapes = new ResourceHandler<ShapeData>();

            RegisterRightTriangle();
            RegisterSquare();
            RegisterStar();
            RegisterRegularPolygon("Morro_Triangle", 3);
            RegisterRegularPolygon("Morro_Circle", 90);
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
        /// Create a new regular polygon, and register its <see cref="ShapeData"/> to be managed by Morro.
        /// </summary>
        /// <param name="name">The name that the shape data being registered will be referenced as.</param>
        /// <param name="totalVertices">The total amount of vertices the regular polygon should have.</param>
        public static void RegisterRegularPolygon(string name, int totalVertices)
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

            RegisterShapeData(name, new ShapeData(vertices.ToArray(), indices.ToArray())); ;
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
                }
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
                }
            );

            RegisterShapeData("Morro_Square", squareData);
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

            RegisterShapeData("Morro_Star", new ShapeData(vertices.ToArray(), indices.ToArray()));
        }
    }
}
