using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class MCircle : MPolygon
    {
        public float Radius
        {
            get => radius;
            set
            {
                radius = value;
                Width = radius * 2;
                Height = radius * 2;
            }
        }

        private float radius;

        public MCircle(float x, float y, float radius) : base(x, y, radius * 2, radius * 2, ShapeType.Circle)
        {
        }

        public void SetCenter()
        {

        }
    }
}
