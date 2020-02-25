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
                Matrix.CreateScale(dimension.Width , dimension.Height, 1) *
                Matrix.CreateScale(transform.Scale) *
                
                Matrix.CreateTranslation(-transform.RotationOffset) *
                Matrix.CreateRotationZ(transform.Rotation) *
                Matrix.CreateTranslation(transform.RotationOffset) *

                Matrix.CreateTranslation(position.X, position.Y, 0) *
                Matrix.CreateTranslation(transform.Translation) *

                Matrix.Identity;
        }
    }
}
