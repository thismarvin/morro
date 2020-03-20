﻿using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class Quad : Polygon
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

        public Quad(float x, float y, float width, float height) : base(x, y, width, height, ShapeType.Square)
        {

        }
    }
}
