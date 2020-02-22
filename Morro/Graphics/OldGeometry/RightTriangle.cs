using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Graphics
{
    public enum RightAnglePositionType
    {
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
    }

    class RightTriangle : Polygon, IConvex
    {
        public RightAnglePositionType RightAnglePosition { get; private set; }

        public RightTriangle(float x, float y, int width, int height, RightAnglePositionType rightAnglePosition, Color color, VertexInformation vertexInformation) : this(x, y, width, height, 0, rightAnglePosition, color, vertexInformation)
        {

        }

        public RightTriangle(float x, float y, int width, int height, float lineWidth, RightAnglePositionType rightAnglePosition, Color color, VertexInformation vertexInformation) : base(x, y, width, height, 3, lineWidth, color, vertexInformation)
        {
            RightAnglePosition = rightAnglePosition;

            CreatePolygon();
        }

        public void SetRightAnglePosition(RightAnglePositionType rightAnglePosition)
        {
            if (RightAnglePosition == rightAnglePosition)
                return;

            RightAnglePosition = rightAnglePosition;

            CreateVertices();
            SendToGraphicsManager();
            //CreateTransformedLineSegments();
        }

        protected override void CreateKey()
        {
            if (LineWidth == 0)
            {
                vertexKey = string.Format(CultureInfo.InvariantCulture, "RightTrianlge: {0}V, {1}RAP, C{2}", initialTotalVertices, (int)RightAnglePosition, Color);
                indexKey = string.Format(CultureInfo.InvariantCulture, "RightTrianlge: {0}V", initialTotalVertices);
            }
            else
            {
                vertexKey = string.Format(CultureInfo.InvariantCulture, "RightTrianlge: {0}V, {1}LW, {2}W, {3}H, {4}RAP, C{5}", initialTotalVertices, LineWidth, Width, Height, (int)RightAnglePosition, Color);
                indexKey = string.Format(CultureInfo.InvariantCulture, "RightTrianlge: {0}V, Hollow", initialTotalVertices);
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
            switch (RightAnglePosition)
            {
                case RightAnglePositionType.TopLeft:
                    vertices = new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(0, 1, 0), Color),
                        new VertexPositionColor(new Vector3(1, 1, 0), Color),
                        new VertexPositionColor(new Vector3(1, 0, 0), Color),
                    };
                    break;

                case RightAnglePositionType.TopRight:
                    vertices = new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(0, 0, 0), Color),
                        new VertexPositionColor(new Vector3(0, 1, 0), Color),
                        new VertexPositionColor(new Vector3(1, 1, 0), Color),
                    };
                    break;

                case RightAnglePositionType.BottomRight:
                    vertices = new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(0, 0, 0), Color),
                        new VertexPositionColor(new Vector3(0, 1, 0), Color),
                        new VertexPositionColor(new Vector3(1, 0, 0), Color),
                    };
                    break;

                case RightAnglePositionType.BottomLeft:
                    vertices = new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(0, 0, 0), Color),
                        new VertexPositionColor(new Vector3(1, 1, 0), Color),
                        new VertexPositionColor(new Vector3(1, 0, 0), Color),
                    };
                    break;
            }
        }

        protected override void CreateHollowVertices()
        {
            float scaledLineWidthX = LineWidth / Width;
            float scaledLineWidthY = LineWidth / Height;

            float theta;
            theta = (float)Math.Atan((float)Height / Width);
            float xOffset = (float)(scaledLineWidthX / Math.Tan(theta)) + (float)(scaledLineWidthX / Math.Sin(theta));
            theta = (float)Math.Atan((float)Width / Height);
            float yOffset = (float)(scaledLineWidthY / Math.Tan(theta)) + (float)(scaledLineWidthY / Math.Sin(theta));
       
            switch (RightAnglePosition)
            {
                case RightAnglePositionType.TopLeft:
                    vertices = new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(xOffset, 1 - scaledLineWidthY, 0), Color),
                        new VertexPositionColor(new Vector3(1 - scaledLineWidthX, 1 - scaledLineWidthY, 0), Color),
                        new VertexPositionColor(new Vector3(1 - scaledLineWidthY, yOffset, 0), Color),

                        new VertexPositionColor(new Vector3(0, 1, 0), Color),
                        new VertexPositionColor(new Vector3(1, 1, 0), Color),
                        new VertexPositionColor(new Vector3(1, 0, 0), Color),
                    };
                    break;

                case RightAnglePositionType.TopRight:
                    vertices = new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(scaledLineWidthX, yOffset, 0), Color),
                        new VertexPositionColor(new Vector3(scaledLineWidthX, 1 - scaledLineWidthY, 0), Color),
                        new VertexPositionColor(new Vector3(1 - xOffset, 1 - scaledLineWidthY, 0), Color),

                        new VertexPositionColor(new Vector3(0, 0, 0), Color),
                        new VertexPositionColor(new Vector3(0, 1, 0), Color),
                        new VertexPositionColor(new Vector3(1, 1, 0), Color),
                    };
                    break;

                case RightAnglePositionType.BottomRight:
                    vertices = new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(scaledLineWidthX, scaledLineWidthY, 0), Color),
                        new VertexPositionColor(new Vector3(scaledLineWidthX, 1 - yOffset, 0), Color),
                        new VertexPositionColor(new Vector3(1 - xOffset, scaledLineWidthY, 0), Color),

                        new VertexPositionColor(new Vector3(0, 0, 0), Color),
                        new VertexPositionColor(new Vector3(0, 1, 0), Color),
                        new VertexPositionColor(new Vector3(1, 0, 0), Color),
                    };
                    break;

                case RightAnglePositionType.BottomLeft:
                    vertices = new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(xOffset, scaledLineWidthY, 0), Color),
                        new VertexPositionColor(new Vector3(1 - scaledLineWidthX, 1 - yOffset, 0), Color),
                        new VertexPositionColor(new Vector3(1 - scaledLineWidthX, scaledLineWidthY, 0), Color),

                        new VertexPositionColor(new Vector3(0, 0, 0), Color),
                        new VertexPositionColor(new Vector3(1, 1, 0), Color),
                        new VertexPositionColor(new Vector3(1, 0, 0), Color),
                    };
                    break;
            }
        }
    }
}
