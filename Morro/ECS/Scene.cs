using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morro.ECS
{
    abstract class Scene
    {
        public bool PartitioningEnabled { get; private set; }
        public PartitionerType PartitionerPreference { get; private set; }
        public Partitioner Partitioner { get; private set; }
        public Camera Camera { get; private set; }
        public Transition EnterTransition { get; set; }
        public Transition ExitTransition { get; set; }
        public string Name { get; private set; }
        public Core.Rectangle SceneBounds { get; private set; }
        public SparseSet EntitiesInView { get; private set; }

        private readonly SparseSet entityRemovalQueue;

        private readonly int queryBuffer;

        private readonly int maximumComponentCount;
        private readonly HashSet<Type> registeredComponents;
        private readonly Dictionary<Type, int> componentLookup;
        private int componentIndex;
        private readonly IComponent[][] data;

        private readonly int maximumSystemCount;
        private readonly HashSet<Type> registeredSystems;
        private readonly MorroSystem[] systems;
        private int systemIndex;

        protected readonly int maximumEntityCount;
        private int nextEntity;
        private readonly SparseSet[] attachedComponents;
        private readonly SparseSet[] attachedSystems;

        public int TotalEntities { get => maximumEntityCount; }

        public Scene(string name, int maximumEntityCount = 100, int maximumComponentCount = 64, int maximumSystemCount = 64)
        {
            Name = name;
            SceneBounds = new Core.Rectangle(0, 0, WindowManager.PixelWidth, WindowManager.PixelHeight);

            PreferQuadtreePartitioner(4);
            queryBuffer = 16;

            Camera = new Camera(Name);
            Camera.SetMovementRestriction(0, 0, SceneBounds.Width, SceneBounds.Height);
            CameraManager.RegisterCamera(Camera);

            EnterTransition = new Pinhole(TransitionType.Enter);
            ExitTransition = new Fade(TransitionType.Exit);

            this.maximumSystemCount = maximumSystemCount;
            this.maximumComponentCount = maximumComponentCount;
            this.maximumEntityCount = maximumEntityCount;

            registeredComponents = new HashSet<Type>();
            componentLookup = new Dictionary<Type, int>();
            data = new IComponent[this.maximumComponentCount][];

            registeredSystems = new HashSet<Type>();
            systems = new MorroSystem[this.maximumSystemCount];

            attachedComponents = new SparseSet[this.maximumEntityCount];
            attachedSystems = new SparseSet[this.maximumEntityCount];

            EntitiesInView = new SparseSet(this.maximumEntityCount);

            entityRemovalQueue = new SparseSet(this.maximumEntityCount);
        }

        #region ECS Stuff

        public void RegisterSystem(MorroSystem system)
        {
            if (systemIndex > maximumSystemCount)
                throw new MorroException("Too many systems have been registered. Consider raising the maximum amount of systems allowed.", new IndexOutOfRangeException());

            if (registeredSystems.Contains(system.GetType()))
                return;

            registeredSystems.Add(system.GetType());
            systems[systemIndex] = system;
            systemIndex++;
        }

        public int CreateEntity(IComponent[] components)
        {
            int entity = AllocateEntity();

            AddComponent(entity, components);

            return entity;
        }

        public void RemoveEntity(int entity)
        {
            entityRemovalQueue.Add((uint)entity);
        }

        public bool EntityContains(int entity, params Type[] components)
        {
            uint componentID;
            for (int i = 0; i < components.Length; i++)
            {
                if (!registeredComponents.Contains(components[i]))
                {
                    return false;
                }

                componentID = (uint)componentLookup[components[i]];
                if (!attachedComponents[entity].Contains(componentID))
                {
                    return false;
                }
            }

            return true;
        }

        public void AddComponent(int entity, params IComponent[] components)
        {
            Type componentType;
            for (int i = 0; i < components.Length; i++)
            {
                componentType = components[i].GetType();
                if (!registeredComponents.Contains(componentType))
                {
                    RegisterComponent(componentType);
                }

                AssignComponent(entity, components[i]);
            }

            AssignSystems(entity);
        }

        public void RemoveComponent(int entity, params Type[] componentTypes)
        {
            for (int i = 0; i < componentTypes.Length; i++)
            {
                if (!registeredComponents.Contains(componentTypes[i]))
                    continue;

                data[componentLookup[componentTypes[i]]][entity] = null;
                attachedComponents[entity].Remove((uint)componentLookup[componentTypes[i]]);

                foreach (uint j in attachedSystems[entity])
                {
                    if (systems[j].RequiredComponents.Contains(componentTypes[i]))
                    {
                        systems[j].RemoveEntity(entity);
                    }
                }
            }
        }

        private void ClearEntity(uint entity)
        {
            foreach (uint i in attachedComponents[entity])
            {
                data[i][entity] = null;
            }

            foreach (uint i in attachedSystems[entity])
            {
                systems[i].RemoveEntity((int)entity);
            }

            attachedComponents[entity].Clear();
            attachedSystems[entity].Clear();
        }

        private int AllocateEntity()
        {
            int entity = nextEntity;

            if (attachedComponents[entity] == null)
            {
                attachedComponents[entity] = new SparseSet(maximumComponentCount);
                attachedSystems[entity] = new SparseSet(maximumSystemCount);
            }
            else
            {
                attachedComponents[entity].Clear();
                attachedSystems[entity].Clear();
            }

            nextEntity = nextEntity + 1 >= maximumEntityCount ? 0 : ++nextEntity;

            return entity;
        }

        private void AssignSystems(int entity)
        {
            for (int i = 0; i < systemIndex; i++)
            {
                if (EntityContains(entity, systems[i].RequiredComponents.ToArray()))
                {
                    systems[i].AddEntity(entity);
                    attachedSystems[entity].Add((uint)i);
                }
            }
        }

        private void AssignComponent(int entity, IComponent component)
        {
            Type componentType = component.GetType();

            data[componentLookup[componentType]][entity] = component;
            attachedComponents[entity].Add((uint)componentLookup[componentType]);
        }

        private void RegisterComponent(Type componentType)
        {
            registeredComponents.Add(componentType);
            componentLookup.Add(componentType, componentIndex);
            data[componentIndex] = new IComponent[maximumEntityCount];

            componentIndex++;
        }

        public ECS.IComponent[] GetData<IComponent>()
        {
            Type componentType = typeof(IComponent);

            if (!registeredComponents.Contains(componentType))
                return new ECS.IComponent[0];

            return data[componentLookup[componentType]];
        }

        public IComponent GetEntityData(int entity, Type type)
        {
            if (!componentLookup.ContainsKey(type))
                return null;

            return data[componentLookup[type]][entity];
        }

        public void UpdateECS()
        {
            SpatialPartitioning();

            for (int i = 0; i < systemIndex; i++)
            {
                systems[i].BeforeUpdate();
                systems[i].Update();
            }

            if (entityRemovalQueue.Count != 0)
            {
                foreach (uint entity in entityRemovalQueue)
                {
                    ClearEntity(entity);
                }
                entityRemovalQueue.Clear();
            }
        }

        public void DrawECS(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < systemIndex; i++)
            {
                systems[i].BeforeDraw(spriteBatch);
                systems[i].Draw(spriteBatch, Camera);
            }
        }

        #endregion

        public SparseSet Query(Core.Rectangle bounds)
        {
            SparseSet result = new SparseSet(maximumEntityCount);

            if (!PartitioningEnabled)
            {
                for (int entity = 0; entity < maximumEntityCount; entity++)
                {
                    if (EntityContains(entity, typeof(CPosition), typeof(CDimension)) || EntityContains(entity, typeof(CPosition)))
                    {
                        result.Add((uint)entity);
                    }
                }
            }

            List<PartitionEntry> queryResult = Partitioner.Query(bounds);

            for (int i = 0; i < queryResult.Count; i++)
            {
                result.Add((uint)queryResult[i].ID);
            }

            return result;
        }

        public bool EntityInView(int entity)
        {
            if (!PartitioningEnabled)
                return true;

            return EntitiesInView.Contains((uint)entity);
        }

        protected void DisablePartitioning()
        {
            if (!PartitioningEnabled)
                return;

            PartitioningEnabled = false;
            PartitionerPreference = PartitionerType.None;
            Partitioner.Clear();
            Partitioner = null;
        }

        /// <summary>
        /// Set the <see cref="PartitionerPreference"/> to <see cref="PartitionerType.Quadtree"/>, and initialize a new <see cref="Quadtree"/>.
        /// </summary>
        /// <param name="capacity">the amount of entiies allowed in each <see cref="Quadtree"/>.</param>
        protected void PreferQuadtreePartitioner(int capacity)
        {
            PartitioningEnabled = true;
            PartitionerPreference = PartitionerType.Quadtree;
            Partitioner = new Quadtree(SceneBounds, capacity);
        }

        /// <summary>
        /// Set the <see cref="PartitionerPreference"/> to <see cref="PartitionerType.Bin"/>, and initialize a new <see cref="Bin"/>.
        /// </summary>
        /// <param name="maximumDimension">the maximum dimension of the entities expected.</param>
        protected void PreferBinPartitioner(int maximumDimension)
        {
            PartitioningEnabled = true;
            PartitionerPreference = PartitionerType.Bin;
            int optimalBinSize = (int)Math.Ceiling(Math.Log(maximumDimension, 2));
            Partitioner = new Bin(SceneBounds, optimalBinSize);
        }

        protected void SetSceneBounds(int width, int height)
        {
            if (SceneBounds.Width == width && SceneBounds.Height == height)
                return;

            SceneBounds = new Core.Rectangle(0, 0, width, height);

            if (PartitioningEnabled)
                Partitioner.SetBoundary(SceneBounds);

            Camera.SetMovementRestriction(0, 0, SceneBounds.Width, SceneBounds.Height);
        }

        protected void SpatialPartitioning()
        {
            if (!PartitioningEnabled)
                return;

            Partitioner.Clear();

            CPosition position;
            CDimension dimension;
            for (int entity = 0; entity < maximumEntityCount; entity++)
            {
                if (attachedComponents[entity] == null)
                    continue;

                if (EntityContains(entity, typeof(CPosition), typeof(CDimension)))
                {
                    position = (CPosition)GetEntityData(entity, typeof(CPosition));
                    dimension = (CDimension)GetEntityData(entity, typeof(CDimension));

                    Partitioner.Insert(new PartitionEntry(entity, new Core.Rectangle(position.X, position.Y, (int)dimension.Width, (int)dimension.Height)));
                }
                else if (EntityContains(entity, typeof(CPosition)))
                {
                    position = (CPosition)GetEntityData(entity, typeof(CPosition));

                    Partitioner.Insert(new PartitionEntry(entity, new Core.Rectangle(position.X, position.Y, 1, 1)));
                }
            }

            EntitiesInView = Query(new Core.Rectangle(Camera.Bounds.X - queryBuffer, Camera.Bounds.Y - queryBuffer, Camera.Bounds.Width + queryBuffer * 2, Camera.Bounds.Height + queryBuffer * 2));
        }

        public abstract void LoadScene();

        public abstract void UnloadScene();

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
