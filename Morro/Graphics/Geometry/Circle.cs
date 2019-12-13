﻿using Microsoft.Xna.Framework;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Graphics
{
    class Circle : RegularPolygon
    {
        public int Radius { get; private set; }

        public Circle(float x, float y, int radius, Color color, VertexInformation vertexInformation) : this(x, y, radius, 0, color, vertexInformation)
        {

        }

        public Circle(float x, float y, int radius, float lineWidth, Color color, VertexInformation vertexInformation) : base(x - radius, y - radius, radius * 2, radius * 2, 90, lineWidth, color, vertexInformation)
        {
            Radius = radius;
        }

        public override void SetLocation(float x, float y)
        {
            base.SetLocation(x - Radius, y - Radius);
        }

        public void SetRadius(int radius)
        {
            Radius = radius;
            SetDimensions(Radius * 2, Radius * 2);
        }

        public bool Intersects(Circle circle)
        {
            float distance = Vector2.Distance(Center, circle.Center);
            return distance < Radius || distance < circle.Radius;
        }

        protected override void CreateKey()
        {
            if (LineWidth == 0)
            {
                vertexKey = string.Format(CultureInfo.InvariantCulture, "Circle: {0}V, C{1}", initialTotalVertices, Color);
                indexKey = string.Format(CultureInfo.InvariantCulture, "Circle: {0}V", initialTotalVertices);
            }
            else
            {
                vertexKey = string.Format(CultureInfo.InvariantCulture, "Circle: {0}V, {1}LW, {2}W, C{3}", initialTotalVertices, LineWidth, Width, Color);
                indexKey = string.Format(CultureInfo.InvariantCulture, "Circle: {0}V, Hollow", initialTotalVertices);
            }
        }
    }
}
