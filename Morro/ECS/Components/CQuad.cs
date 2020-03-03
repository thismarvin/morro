using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class CQuad : IComponent
    {
        public Matrix Transform { get; private set; }

        public CQuad(CPosition position, CDimension dimension, CTransform transform)
        {
            SetTransform(position, dimension, transform);
        }

        public void SetTransform(CPosition position, CDimension dimension, CTransform transform)
        {
            Transform =
                Matrix.CreateScale(dimension.Width * transform.Scale.X, dimension.Height * transform.Scale.Y, 1 * transform.Scale.Z) *

                Matrix.CreateTranslation(-transform.RotationOffset) *
                Matrix.CreateRotationZ(transform.Rotation) *

                Matrix.CreateTranslation(position.X + transform.Translation.X + transform.RotationOffset.X, position.Y + transform.Translation.Y + transform.RotationOffset.Y, 0 + transform.Translation.Z + transform.RotationOffset.Z) *

                Matrix.Identity;
        }
    }
}
