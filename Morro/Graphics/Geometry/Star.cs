using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


namespace Morro.Graphics
{
    class Star : Polygon
    {
        protected float angleIncrement;

        public Star(float x, float y, int width, int height, Color color, VertexInformation vertexInformation) : this(x, y, width, height, 0, color, vertexInformation)
        {

        }

        public Star(float x, float y, int width, int height, float lineWidth, Color color, VertexInformation vertexInformation) : base(x, y, width, height, 10, lineWidth, color, vertexInformation)
        {
            angleIncrement = MathHelper.TwoPi / initialTotalVertices;

            CreatePolygon();
        }

        protected override void CreateKey()
        {
            if (LineWidth == 0)
            {
                vertexKey = string.Format(CultureInfo.InvariantCulture, "Star: {0}V, C{1}", initialTotalVertices, Color);
                indexKey = string.Format(CultureInfo.InvariantCulture, "Star: {0}V", initialTotalVertices);
            }
            else
            {
                vertexKey = string.Format(CultureInfo.InvariantCulture, "Star: {0}V, {1}LW, {2}W, C{3}", initialTotalVertices, LineWidth, Width, Color);
                indexKey = string.Format(CultureInfo.InvariantCulture, "Star: {0}V, Hollow", initialTotalVertices);
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
            int alternate = 0;
            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                if (alternate % 2 == 0)
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(0.5f + (float)Math.Cos(i) * 0.25f, 0.5f + (float)Math.Sin(i) * 0.25f, 0), Color);
                else
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(0.5f + (float)Math.Cos(i) * 0.5f, 0.5f + (float)Math.Sin(i) * 0.5f, 0), Color);

                alternate++;

                if (vertexIndex >= vertices.Length)
                    break;
            }
        }

        protected override void CreateHollowVertices()
        {
            float theta;
            theta = MathHelper.TwoPi / initialTotalVertices;
            theta = MathHelper.Pi - theta;
            theta /= 2;

            float scaledLineWidth = (LineWidth / Width) / (float)Math.Sin(theta);
            int vertexIndex = 0;
            int alternate = 0;

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                if (alternate % 2 == 0)
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(0.5f + (float)Math.Cos(i) * (0.25f - scaledLineWidth / 2), 0.5f + (float)Math.Sin(i) * (0.25f - scaledLineWidth / 2), 0), Color);
                else
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(0.5f + (float)Math.Cos(i) * (0.5f - scaledLineWidth), 0.5f + (float)Math.Sin(i) * (0.5f - scaledLineWidth), 0), Color);

                alternate++;

                if (vertexIndex >= vertices.Length / 2)
                    break;
            }

            for (float i = MathHelper.TwoPi; i >= 0; i -= angleIncrement)
            {
                if (alternate % 2 == 0)
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(0.5f + (float)Math.Cos(i) * 0.25f, 0.5f + (float)Math.Sin(i) * 0.25f, 0), Color);
                else
                    vertices[vertexIndex++] = new VertexPositionColor(new Vector3(0.5f + (float)Math.Cos(i) * 0.5f, 0.5f + (float)Math.Sin(i) * 0.5f, 0), Color);

                alternate++;

                if (vertexIndex >= vertices.Length)
                    break;
            }
        }
    }
}
