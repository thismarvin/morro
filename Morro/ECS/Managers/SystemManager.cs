using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Morro.ECS
{
    class SystemManager
    {
        public int Capacity { get; private set; }
        public int TotalSystemsRegistered { get; private set; }
        public bool DisableAsynchronousUpdates { get; set; }
        public MorroSystem[] Systems { get; private set; }

        private readonly HashSet<Type> registeredSystems;
        private readonly Dictionary<Type, int> systemLookup;
        private MorroSystem[][] updateGroups;

        public SystemManager(int capacity)
        {
            Capacity = capacity;

            Systems = new MorroSystem[Capacity];
            registeredSystems = new HashSet<Type>();
            systemLookup = new Dictionary<Type, int>();
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

            if (system is IUpdateableSystem)
            {
                CreateUpdateGroups();
            }
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
            if (DisableAsynchronousUpdates)
            {
                SynchronousUpdate();
            }
            else
            {
                AsynchronousUpdate();
            }
        }

        public void Draw(Camera camera)
        {
            for (int i = 0; i < TotalSystemsRegistered; i++)
            {
                if (Systems[i].Enabled && Systems[i] is IDrawableSystem)
                {
                    ((IDrawableSystem)Systems[i]).Draw(camera);
                }
            }
        }

        private void CreateUpdateGroups()
        {
            int totalCanidates;
            int groupIndex = -1;
            HashSet<MorroSystem> canidates = new HashSet<MorroSystem>();
            HashSet<Type> registered = new HashSet<Type>();
            List<MorroSystem> removalQueue = new List<MorroSystem>();
            List<List<MorroSystem>> groups = new List<List<MorroSystem>>(TotalSystemsRegistered);

            SetupCanidates();
            ProcessCanidates();
            HandleDependencies();
            FinalizeResults();

            void SetupCanidates()
            {
                for (int i = 0; i < TotalSystemsRegistered; i++)
                {
                    if (Systems[i] is IUpdateableSystem)
                    {
                        canidates.Add(Systems[i]);
                    }
                }

                totalCanidates = canidates.Count;
            }

            void ProcessCanidates()
            {
                CreateNewGroup();

                foreach (MorroSystem system in canidates)
                {
                    if (system.Dependencies.Count == 0)
                    {
                        ProcessSystem(system);
                    }
                }
                ClearRemovalQueue();
            }

            void HandleDependencies()
            {
                bool done = false;
                CreateNewGroup();

                while (!done)
                {
                    done = true;

                    foreach (MorroSystem system in canidates)
                    {
                        foreach (Type dependency in system.Dependencies)
                        {
                            if (registered.Contains(dependency))
                            {
                                ProcessSystem(system);
                                done = false;
                            }
                        }
                    }
                    ClearRemovalQueue();

                    if (!done && canidates.Count != 0)
                    {
                        CreateNewGroup();
                    }
                }
            }

            void FinalizeResults()
            {
                updateGroups = new MorroSystem[groups.Count][];
                for (int i = 0; i < groups.Count; i++)
                {
                    updateGroups[i] = new MorroSystem[groups[i].Count];
                    for (int j = 0; j < groups[i].Count; j++)
                    {
                        updateGroups[i][j] = groups[i][j];
                    }
                }
            }

            void CreateNewGroup()
            {
                groups.Add(new List<MorroSystem>(totalCanidates));
                groupIndex++;
            }

            void ProcessSystem(MorroSystem system)
            {
                groups[groupIndex].Add(system);
                removalQueue.Add(system);
                registered.Add(system.GetType());
            }

            void ClearRemovalQueue()
            {
                for (int i = 0; i < removalQueue.Count; i++)
                {
                    canidates.Remove(removalQueue[i]);
                }
                removalQueue.Clear();
            }
        }

        private void SynchronousUpdate()
        {
            for (int i = 0; i < updateGroups.Length; i++)
            {
                for (int j = 0; j < updateGroups[i].Length; j++)
                {
                    if (updateGroups[i][j].Enabled)
                    {
                        ((IUpdateableSystem)updateGroups[i][j]).Update();
                    }
                }
            }
        }

        private void AsynchronousUpdate()
        {
            for (int i = 0; i < updateGroups.Length; i++)
            {
                Task.WaitAll(DivideSystemsIntoTasks(i));
            }

            Task[] DivideSystemsIntoTasks(int groupIndex)
            {
                int taskIndex = 0;
                int totalTasks = updateGroups[groupIndex].Length;
                Task[] tasks = new Task[totalTasks];

                for (int i = 0; i < totalTasks; i++)
                {
                    if (updateGroups[groupIndex][i].Enabled)
                    {
                        tasks[taskIndex++] = CreateTask(groupIndex, i);
                    }
                }

                return tasks;
            }

            Task CreateTask(int groupIndex, int systemIndex)
            {
                return Task.Run(() =>
                {
                    ((IUpdateableSystem)updateGroups[groupIndex][systemIndex]).Update();
                });
            }
        }
    }
}
