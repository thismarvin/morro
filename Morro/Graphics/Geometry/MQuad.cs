using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class MQuad : MPolygon
    {
        public float LineWidth
        {
            get => lineWidth;
            set
            {
                lineWidth = value;
                ShapeData = GeometryManager.CreateHollowSquare(Width, Height, lineWidth);
            }
        }

        private float lineWidth;

        public MQuad(float x, float y, float width, float height) : base(x, y, width, height, ShapeType.Square)
        {

        }
    }
}
