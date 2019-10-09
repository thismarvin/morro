using Microsoft.Xna.Framework;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class AABB : Quad
    {
        public AABB(float x, float y, int width, int height, Color color, VertexInformation vertexInformation) : this(x, y, width, height, 0, color, vertexInformation)
        {

        }

        public AABB(float x, float y, int width, int height, float lineWidth, Color color, VertexInformation vertexInformation) : base(x, y, width, height, lineWidth, color, vertexInformation)
        {
            CreatePolygon();
        }

        public override void SetRotationOffset(float xOffset, float yOffset)
        {
            
        }

        public override void SetRotation(float rotation)
        {
            
        }

        public override void Rotate(float angle)
        {
            
        }

        protected override void CreateTransformedVertices()
        {
            TransformedVertices = new Vector2[totalVertices];

            if (Filled)
            {
                TransformedVertices[0] = new Vector2(Bounds.Left, Bounds.Bottom);
                TransformedVertices[1] = new Vector2(Bounds.Right, Bounds.Bottom);
                TransformedVertices[2] = new Vector2(Bounds.Right, Bounds.Top);
                TransformedVertices[3] = new Vector2(Bounds.Left, Bounds.Top);
            }
            else
            {
                TransformedVertices[0] = new Vector2(Bounds.Left + LineWidth, Bounds.Bottom - LineWidth);
                TransformedVertices[1] = new Vector2(Bounds.Right - LineWidth, Bounds.Bottom - LineWidth);
                TransformedVertices[2] = new Vector2(Bounds.Right - LineWidth, Bounds.Top + LineWidth);
                TransformedVertices[3] = new Vector2(Bounds.Left + LineWidth, Bounds.Top + LineWidth);
                TransformedVertices[4] = new Vector2(Bounds.Left, Bounds.Bottom);
                TransformedVertices[5] = new Vector2(Bounds.Right, Bounds.Bottom);
                TransformedVertices[6] = new Vector2(Bounds.Right, Bounds.Top);
                TransformedVertices[7] = new Vector2(Bounds.Left, Bounds.Top);
            }
        }
    }
}
