using Microsoft.Xna.Framework.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    /// <summary>
    /// An abstraction of a <see cref="MorroSystem"/> that can process <see cref="IComponent"/> data and preform draw logic every frame.
    /// </summary>
    abstract class DrawSystem : MorroSystem
    {
        /// <summary>
        /// Create a <see cref="MorroSystem"/> that will process <see cref="IComponent"/> data and preform draw logic every frame.
        /// </summary>
        /// <param name="scene">The scene this system will exist in.</param>
        internal DrawSystem(Scene scene) : base(scene)
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
