using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class CPosition : IComponent
    {
        public float X { get; set; }
        public float Y { get; set; }

        public CPosition(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
