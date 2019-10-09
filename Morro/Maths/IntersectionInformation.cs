using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths
{
    struct IntersectionInformation
    {
        public float T { get; private set; }
        public float U { get; private set; }
        public bool Intersected { get; private set; }
        public Vector2 IntersectionPoint { get; private set; }

        public IntersectionInformation(float t, float u, Vector2 intersectionPoint)
        {
            T = t;
            U = u;
            Intersected = 0 <= T && T <= 1 && 0 <= U && U <= 1;
            IntersectionPoint = intersectionPoint;
        }
    }
}
