using Microsoft.Xna.Framework;
using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Components
{
    class CPredator : IComponent
    {
        public Vector2 Target { get; set; }
        public bool Seeking { get; set; }
    }
}
