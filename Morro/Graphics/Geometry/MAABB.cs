using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class MAABB : MQuad
    {
        public override float Rotation { get => base.Rotation; set => base.Rotation = 0; }

        public MAABB(float x, float y, float width, float height) : base(x, y, width, height)
        {

        }
    }
}
