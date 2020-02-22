using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class CAABB : IComponent
    {
        public Matrix Transform { get; private set; }

        public CAABB(CPosition position, CDimension dimension)
        {
            Transform =                 
                Matrix.CreateScale(dimension.Width, dimension.Height, 1) *
                Matrix.CreateTranslation(position.X, position.Y, 0) *
                Matrix.Identity;
        }

        public void SetTransform(CPosition position, CDimension dimension)
        {
            Transform =
                Matrix.CreateScale(dimension.Width, dimension.Height, 1) *
                Matrix.CreateTranslation(position.X, position.Y, 0) *
                Matrix.Identity;
        }
    }
}
