using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Graphics
{
    class RegularPolygon : Polygon, IConvex
    {
        protected float angleIncrement;

        public RegularPolygon(float x, float y, int width, int height, int vertices, Color color, VertexInformation vertexInformation) : this(x, y, width, height, vertices, 0, color, vertexInformation)
        {

        }

        public RegularPolygon(float x, float y, int width, int height, int vertices, float lineWidth, Color color, VertexInformation vertexInformation) : base(x, y, width, height, vertices, lineWidth, color, vertexInformation)
        {
            angleIncrement = MathHelper.TwoPi / initialTotalVertices;

            CreatePolygon();
        }

        protected override void CreateKey()
        {
            if (LineWidth == 0)
            {
                vertexKey = string.Format(CultureInfo.InvariantCulture, "RegularPolygon: {0}V, C{1}", initialTotalVertices, Color);
                indexKey = string.Format(CultureInfo.InvariantCulture, "RegularPolygon: {0}V", initialTotalVertices);
            }
            else
            {
                vertexKey = string.Format(CultureInfo.InvariantCulture, "RegularPolygon: {0}V, {1}LW, {2}W, C{3}", initialTotalVertices, LineWidth, Width, Color);
                indexKey = string.Format(CultureInfo.InvariantCulture, "RegularPolygon: {0}V, Hollow", initialTotalVertices);
            }
        }

        protected override void CreateFilledIndices()
        {
            int j = 1;
            for (int i = 0; i < totalIndices; i += 3)
            {
                indices[i] = 0;
                indices[i + 1] = (short)(j + 1);
                indices[i + 2] = (short)(j);
                j++;
            }
        }

        protected override void CreateHollowIndices()
        {
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
        }

        protected override void CreateFilledVertices()
        {
            int vertexIndex = 0;
            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                vertices[vertexIndex++] = new VertexPositionColor(new Vector3(0.5f + (float)Math.Cos(i) * 0.5f, 0.5f + (float)Math.Sin(i) * 0.5f, 0), Color);

                if (vertexIndex >= vertices.Length)
                    break;
            }
        }

        protected override void CreateHollowVertices()
        {
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

            float scaledLineWidth = (LineWidth / Width) / (float)Math.Sin(theta);
            int vertexIndex = 0;

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                vertices[vertexIndex++] = new VertexPositionColor(new Vector3(0.5f + (float)Math.Cos(i) * (0.5f - scaledLineWidth), 0.5f + (float)Math.Sin(i) * (0.5f - scaledLineWidth), 0), Color);

                if (vertexIndex >= vertices.Length / 2)
                    break;
            }

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                vertices[vertexIndex++] = new VertexPositionColor(new Vector3(0.5f + (float)Math.Cos(i) * 0.5f, 0.5f + (float)Math.Sin(i) * 0.5f, 0), Color);

                if (vertexIndex >= vertices.Length)
                    break;
            }
        }
    }
}
