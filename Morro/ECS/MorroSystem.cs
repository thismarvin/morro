using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Morro.ECS
{
    /// <summary>
    /// Systems process a particular set of <see cref="IComponent"/> data, and perform specialized logic on said data.
    /// </summary>
    abstract class MorroSystem : IMorroSystem
    {
        public bool Enabled { get; set; }
        public HashSet<Type> RequiredComponents { get; private set; }
        public HashSet<Type> BlacklistedComponents { get; private set; }
        public HashSet<Type> Dependencies { get; private set; }
        protected internal HashSet<int> Entities { get; private set; }

        protected internal int[] EntitiesAsArray
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

        private readonly int[] entitiesAsArray;
        private bool entityDataChanged;

        internal MorroSystem(Scene scene)
        {
            RequiredComponents = new HashSet<Type>();
            BlacklistedComponents = new HashSet<Type>();
            Dependencies = new HashSet<Type>();
            Entities = new HashSet<int>();
            entitiesAsArray = new int[scene.EntityCapacity];

            Enabled = true;
            this.scene = scene;
        }

        /// <summary>
        /// Initialize a set of <see cref="IComponent"/> types that all entities associated with this system must have.
        /// </summary>
        /// <param name="components">The types of <see cref="IComponent"/> this system will require before associating with an entity.</param>
        public void Require(params Type[] components)
        {
            RequiredComponents.Clear();

            for (int i = 0; i < components.Length; i++)
            {
                RequiredComponents.Add(components[i]);
            }
        }

        /// <summary>
        /// Initialize a set of <see cref="IComponent"/> types this system must avoid before associating itself with an entity.
        /// </summary>
        /// <param name="components">The types of <see cref="IComponent"/> this system will avoid during initialization.</param>
        public void Avoid(params Type[] components)
        {
            BlacklistedComponents.Clear();

            for (int i = 0; i < components.Length; i++)
            {
                BlacklistedComponents.Add(components[i]);
            }
        }

        /// <summary>
        /// Initialize a set of <see cref="MorroSystem"/> types this system depends on running first before this system can run.
        /// </summary>
        /// <param name="systems">The types of <see cref="MorroSystem"/> this systems depends on running first.</param>
        public void Depend(params Type[] systems)
        {
            Dependencies.Clear();

            for (int i = 0; i < systems.Length; i++)
            {
                Dependencies.Add(systems[i]);
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
