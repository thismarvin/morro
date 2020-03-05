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
        public HashSet<Type> BlacklistedComponents { get; private set; }
        protected HashSet<int> Entities { get; private set; }
        protected int[] EntitiesAsArray
        {
            get
            {
                if (!entityDataChanged)
                {
                    return entitiesAsArray;
                }
                else
                {
                    entityDataChanged = false;

                    int entityIndex = 0;

                    foreach (int entity in Entities)
                    {
                        entitiesAsArray[entityIndex++] = entity;
                    }

                    return entitiesAsArray;
                }
            }
        }

        protected Scene scene;

        private int[] entitiesAsArray;
        private bool entityDataChanged;

        internal MorroSystem(Scene scene)
        {
            RequiredComponents = new HashSet<Type>();
            BlacklistedComponents = new HashSet<Type>();
            Entities = new HashSet<int>();
            entitiesAsArray = new int[scene.EntityCapacity];

            Enabled = true;
            this.scene = scene;
        }

        /// <summary>
        /// Initialize a set of <see cref="IComponent"/> types that all entities associated with this system must have.
        /// </summary>
        /// <param name="components"></param>
        public void Require(params Type[] components)
        {
            RequiredComponents.Clear();

            for (int i = 0; i < components.Length; i++)
            {
                RequiredComponents.Add(components[i]);
            }
        }

        /// <summary>
        /// Initialize a set of <see cref="IComponent"/> types this system should avoid before associating itself with an entity.
        /// </summary>
        /// <param name="components"></param>
        public void Avoid(params Type[] components)
        {
            BlacklistedComponents.Clear();

            for (int i = 0; i < components.Length; i++)
            {
                BlacklistedComponents.Add(components[i]);
            }
        }

        internal void AddEntity(int entity)
        {
            if (Entities.Contains(entity))
                return;

            Entities.Add(entity);
            entityDataChanged = true;
        }

        internal void RemoveEntity(int entity)
        {
            if (!Entities.Contains(entity))
                return;

            Entities.Remove(entity);
            entityDataChanged = true;
        }
    }
}
