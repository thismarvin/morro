using Microsoft.Xna.Framework;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths
{
    interface IShape
    {
        RectangleF Bounds { get; set; }
        Vector2[] Vertices { get; set; }
        LineSegment[] LineSegments { get; set; }
    }
}
