using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class MQuad : MPolygon
    {
        public MQuad(float x, float y, float width, float height) : base(x, y, width, height, ShapeType.Square)
        {

        }
    }
}
