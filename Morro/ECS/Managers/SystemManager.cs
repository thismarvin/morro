using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class SystemManager
    {
        public int Capacity { get; private set; }
        public int TotalSystemsRegistered { get; private set; }
        public MorroSystem[] Systems { get; private set; }

        private readonly HashSet<Type> registeredSystems;
        private readonly Dictionary<Type, int> systemLookup;

        public SystemManager(int capacity)
        {
            Capacity = capacity;

            registeredSystems = new HashSet<Type>();
            systemLookup = new Dictionary<Type, int>();
            Systems = new MorroSystem[Capacity];
        }

        public void RegisterSystem(MorroSystem system)
        {
            if (TotalSystemsRegistered > Capacity)
                throw new MorroException("Too many systems have been registered. Consider raising the capacity of systems allowed.", new IndexOutOfRangeException());

            Type systemType = system.GetType();

            if (registeredSystems.Contains(systemType))
                return;

            registeredSystems.Add(systemType);
            systemLookup.Add(systemType, TotalSystemsRegistered);
            Systems[TotalSystemsRegistered] = system;
            TotalSystemsRegistered++;
        }

        public T GetSystem<T>() where T : MorroSystem
        {
            Type systemType = typeof(T);

            if (!registeredSystems.Contains(systemType))
                return default;

            return (T)Systems[systemLookup[systemType]];
        }

        public void Update()
        {
            for (int i = 0; i < TotalSystemsRegistered; i++)
            {
                if (Systems[i].Enabled && Systems[i] is UpdateSystem)
                {
                    ((UpdateSystem)Systems[i]).Update();
                }
            }
        }

        public void Draw(Camera camera)
        {
            for (int i = 0; i < TotalSystemsRegistered; i++)
            {
                if (Systems[i].Enabled && Systems[i] is DrawSystem)
                {
                    ((DrawSystem)Systems[i]).Draw(camera);
                }
            }
        }
    }
}
