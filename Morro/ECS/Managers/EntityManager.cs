using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class EntityManager
    {
        public int Capacity { get; private set; }

        private readonly SparseSet[] attachedComponents;
        private readonly SparseSet[] attachedSystems;
        private int nextEntity;

        private readonly SystemManager systemManager;
        private readonly ComponentManager componentManager;

        public EntityManager(int capacity, SystemManager systemManager, ComponentManager componentManager)
        {
            Capacity = capacity;

            attachedComponents = new SparseSet[Capacity];
            attachedSystems = new SparseSet[Capacity];

            this.systemManager = systemManager;
            this.componentManager = componentManager;
        }

        public int AllocateEntity(int componentCapacity, int systemCapacity)
        {
            int entity = nextEntity;

            if (attachedComponents[entity] == null)
            {
                attachedComponents[entity] = new SparseSet(componentCapacity);
                attachedSystems[entity] = new SparseSet(systemCapacity);
            }
            else
            {
                attachedComponents[entity].Clear();
                attachedSystems[entity].Clear();
            }

            nextEntity = nextEntity + 1 >= Capacity ? 0 : ++nextEntity;

            return entity;
        }

        public void ClearEntity(int entity)
        {
            foreach (uint i in attachedComponents[entity])
            {
                componentManager.Data[i][entity] = null;
            }

            foreach (uint i in attachedSystems[entity])
            {
                systemManager.Systems[i].RemoveEntity(entity);
            }

            attachedComponents[entity].Clear();
            attachedSystems[entity].Clear();
        }

        public void AddComponent(int entity, params IComponent[] components)
        {
            componentManager.RegisterComponent(components);

            AssignComponents(entity, components);
            AssignSystems(entity);
        }

        public void RemoveComponent(int entity, params Type[] componentTypes)
        {
            int componentID;
            for (int i = 0; i < componentTypes.Length; i++)
            {
                componentID = componentManager.GetComponentID(componentTypes[i]);

                if (componentID == -1)
                    continue;

                componentManager.Data[componentID][entity] = null;
                attachedComponents[entity].Remove((uint)componentID);

                foreach (uint j in attachedSystems[entity])
                {
                    if (systemManager.Systems[j].RequiredComponents.Contains(componentTypes[i]))
                    {
                        systemManager.Systems[j].RemoveEntity(entity);
                    }
                }
            }
        }

        public bool EntityContains(int entity, HashSet<Type> components)
        {
            if (components.Count == 0)
                return false;

            int componentID;
            foreach (Type component in components)
            {
                componentID = componentManager.GetComponentID(component);

                if (componentID == -1 || !attachedComponents[entity].Contains((uint)componentID))
                    return false;
            }

            return true;
        }

        public bool EntityContains(int entity, Type[] components)
        {
            if (components.Length == 0)
                return false;

            int componentID;
            for (int i = 0; i < components.Length; i++)
            {
                componentID = componentManager.GetComponentID(components[i]);

                if (componentID == -1 || !attachedComponents[entity].Contains((uint)componentID))
                    return false;
            }

            return true;
        }

        private void AssignComponents(int entity, IComponent[] components)
        {
            int componentID;
            for (int i = 0; i < components.Length; i++)
            {
                componentID = componentManager.GetComponentID(components[i]);

                if (componentID == -1)
                    continue;

                componentManager.Data[componentID][entity] = components[i];
                attachedComponents[entity].Add((uint)componentID);
            }
        }

        private void AssignSystems(int entity)
        {
            for (int i = 0; i < systemManager.TotalSystemsRegistered; i++)
            {
                systemManager.Systems[i].RemoveEntity(entity);

                if (EntityContains(entity, systemManager.Systems[i].RequiredComponents) && !EntityContains(entity, systemManager.Systems[i].BlacklistedComponents))
                {
                    systemManager.Systems[i].AddEntity(entity);
                    attachedSystems[entity].Add((uint)i);
                }
            }
        }
    }
}
