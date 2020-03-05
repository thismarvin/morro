using Microsoft.Xna.Framework.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    abstract class DrawSystem : MorroSystem
    {
        public DrawSystem(Scene scene) : base(scene)
        {
        }

        public abstract void DrawEntity(int entity, Camera camera);

        public virtual void Draw(Camera camera)
        {
            foreach (int entity in Entities)
            {
                DrawEntity(entity, camera);
            }
        }
    }
}
