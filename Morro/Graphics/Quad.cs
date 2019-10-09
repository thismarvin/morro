using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class Quad : Polygon, IConvex
    {
        public Quad(float x, float y, int width, int height, Color color, VertexInformation vertexInformation) : this(x, y, width, height, 0, color, vertexInformation)
        {

        }

        public Quad(float x, float y, int width, int height, float lineWidth, Color color, VertexInformation vertexInformation) : base(x, y, width, height, 4, lineWidth, color, vertexInformation)
        {
            CreatePolygon();
        }

        public override bool Intersects(Polygon polygon)
        {
            if (polygon is Quad)
                return Bounds.Intersects(polygon.Bounds);
            else
                return base.Intersects(polygon);               
        }

        protected override void CreateKey()
        {
            if (LineWidth == 0)
            {
                vertexKey = string.Format("Quad: {0}V, C{1}", initialTotalVertices, Color);
                indexKey = string.Format("Quad: {0}V", initialTotalVertices);
            }
            else
            {
                vertexKey = string.Format("Quad: {0}V, {1}LW, {2}W, {3}H, C{4}", initialTotalVertices, LineWidth, Width, Height, Color);
                indexKey = string.Format("Quad: {0}V, Hollow", initialTotalVertices);
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
            vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(0, 0, 0), Color),
                new VertexPositionColor(new Vector3(0, 1, 0), Color),
                new VertexPositionColor(new Vector3(1, 1, 0), Color),
                new VertexPositionColor(new Vector3(1, 0, 0), Color),
            };
        }

        protected override void CreateHollowVertices()
        {
            float scaledLineWidthX = LineWidth / Width;
            float scaledLineWidthY = LineWidth / Height;

            vertices = new VertexPositionColor[]
            {
                new VertexPositionColor(new Vector3(scaledLineWidthX, scaledLineWidthY, 0), Color),
                new VertexPositionColor(new Vector3(scaledLineWidthX, 1 - scaledLineWidthY, 0), Color),
                new VertexPositionColor(new Vector3(1 - scaledLineWidthX, 1 - scaledLineWidthY, 0), Color),
                new VertexPositionColor(new Vector3(1 - scaledLineWidthX, scaledLineWidthY, 0), Color),

                new VertexPositionColor(new Vector3(0, 0, 0), Color),
                new VertexPositionColor(new Vector3(0, 1, 0), Color),
                new VertexPositionColor(new Vector3(1, 1, 0), Color),
                new VertexPositionColor(new Vector3(1, 0, 0), Color),
            };
        }
    }
}
