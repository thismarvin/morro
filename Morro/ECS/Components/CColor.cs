using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class CColor : IComponent
    {
        public Color Color { get; set; }

        public CColor()
        {

        }

        public CColor(Color color)
        {
            Color = color;
        }
    }
}
