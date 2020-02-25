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
        public static ResourceHandler<ShapeData> Shapes;

        static Geometry()
        {
            Shapes = new ResourceHandler<ShapeData>();

            RegisterRightTriangle();
            RegisterSquare();
            RegisterStar();
            RegisterRegularPolygon("Morro_Triangle", 3);
            RegisterRegularPolygon("Morro_Circle", 90);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="totalVertices"></param>
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

            Shapes.Register(name, new ShapeData(vertices, indices));
        }

        private static void RegisterRightTriangle()
        {
            ShapeData rightTriangleData = new ShapeData
            (
                new List<Vector3>()
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 1, 0),
                },
                new List<short>()
                {
                    0, 1, 2,
                }
            );

            Shapes.Register("Morro_RightTriangle", rightTriangleData);
        }

        private static void RegisterSquare()
        {
            ShapeData squareData = new ShapeData
            (
                new List<Vector3>()
                {
                    new Vector3(0, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(0, 1, 0),
                },
                new List<short>()
                {
                    0, 1, 2,
                    0, 2, 3
                }
            );

            Shapes.Register("Morro_Square", squareData);
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

            Shapes.Register("Morro_Star", new ShapeData(vertices, indices));
        }
    }
}
