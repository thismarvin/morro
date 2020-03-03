using Microsoft.Xna.Framework.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Morro.ECS
{
    abstract class MorroSystem
    {
        public bool Enabled { get; set; }
        public HashSet<Type> RequiredComponents { get; private set; }
        public HashSet<int> Entities { get; private set; }

        protected Scene scene;

        protected List<int> entitiesAsList;

        public MorroSystem(Scene scene)
        {
            RequiredComponents = new HashSet<Type>();
            Entities = new HashSet<int>();
            entitiesAsList = new List<int>();
            Enabled = true;

            this.scene = scene;
        }

        public void Require(params Type[] components)
        {
            RequiredComponents.Clear();

            for (int i = 0; i < components.Length; i++)
            {
                RequiredComponents.Add(components[i]);
            }
        }

        internal void AddEntity(int entity)
        {
            if (Entities.Contains(entity))
                return;

            Entities.Add(entity);
            entitiesAsList.Add(entity);
        }

        internal void RemoveEntity(int entity)
        {
            Entities.Remove(entity);
            entitiesAsList.Remove(entity);
        }
    }
}
