using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    abstract class System
    {
        public string Name { get; private set; }

        private readonly ComponentBag requiredComponents;

        protected List<int> registeredEntities;

        public System(string name)
        {
            Name = name.ToLowerInvariant();

            requiredComponents = new ComponentBag();

            registeredEntities = new List<int>();
        }

        public void Require(params string[] componetNames)
        {
            for (int i = 0; i < componetNames.Length; i++)
            {
                requiredComponents.AddComponent(componetNames);
            }
        }

        public void RegisterEntity(Scene scene, int entity)
        {
            if (!scene.Lentities[entity].Contains(requiredComponents))
                return;

            registeredEntities.Add(entity);
        }

        public abstract void Update(Scene scene);
        public abstract void Draw(SpriteBatch spriteBatch, Scene scene);
    }
}
