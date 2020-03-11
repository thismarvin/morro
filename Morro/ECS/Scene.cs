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

        /// <summary>
        /// By default all registered <see cref="MorroSystem"/>'s are ran asyncronously.
        /// If this functionality is disabled, systems will run in the order they were registered.
        /// </summary>
        public bool AsynchronousSystemsEnabled
        {
            get => !systemManager.DisableAsynchronousUpdates;
            set => systemManager.DisableAsynchronousUpdates = !value;
        }

        private PartitionerType partitionerPreference;
        private Bin bin;

        public Camera Camera { get; private set; }
        public Transition EnterTransition { get; set; }
        public Transition ExitTransition { get; set; }
        public string Name { get; private set; }
        public Core.Rectangle SceneBounds { get; private set; }

        public int SystemCapacity { get => systemManager.Capacity; }
        public int ComponentCapacity { get => componentManager.Capacity; }
        public int EntityCapacity { get => entityManager.Capacity; }

        private readonly SystemManager systemManager;
        private readonly ComponentManager componentManager;
        private readonly EntityManager entityManager;

        private readonly SparseSet entityRemovalQueue;
        private SparseSet entitiesInView;

        private readonly int queryBuffer;

        public int EntityCount { get => entityManager.EntityCount; }


        public Scene(string name, int entityCapacity = 100, int componentCapacity = 64, int systemCapacity = 64)
        {
            Name = name;
            SceneBounds = new Core.Rectangle(0, 0, WindowManager.PixelWidth, WindowManager.PixelHeight);

            queryBuffer = 64;

            Camera = new Camera(Name);
            Camera.SetMovementRestriction(0, 0, SceneBounds.Width, SceneBounds.Height);
            CameraManager.RegisterCamera(Camera);

            EnterTransition = new Pinhole(TransitionType.Enter);
            ExitTransition = new Fade(TransitionType.Exit);

            systemManager = new SystemManager(systemCapacity);
            componentManager = new ComponentManager(componentCapacity, entityCapacity);
            entityManager = new EntityManager(entityCapacity, systemManager, componentManager);

            entitiesInView = new SparseSet(EntityCapacity);
            entityRemovalQueue = new SparseSet(EntityCapacity);
        }

        #region ECS Stuff
        public void RegisterSystem(MorroSystem morroSystem)
        {
            systemManager.RegisterSystem(morroSystem);
        }

        public int CreateEntity(IComponent[] components)
        {
            int entity = entityManager.AllocateEntity();
            entityManager.AddComponent(entity, components);

            return entity;
        }

        public void RemoveEntity(int entity)
        {
            entityRemovalQueue.Add((uint)entity);
        }

        public bool EntityContains(int entity, params Type[] components)
        {
            return entityManager.EntityContains(entity, components);
        }

        public void RemoveComponent(int entity, params Type[] componentTypes)
        {
            entityManager.RemoveComponent(entity, componentTypes);
        }

        private void ClearEntity(int entity)
        {
            entityManager.ClearEntity(entity);
        }

        public IComponent[] GetData<T>() where T : IComponent
        {
            return componentManager.GetData<T>();
        }

        public T GetData<T>(int entity) where T : IComponent
        {
            return componentManager.GetData<T>(entity);
        }

        public T GetSystem<T>() where T : MorroSystem
        {
            return systemManager.GetSystem<T>();
        }

        protected void UpdateECS()
        {
            systemManager.Update();

            if (entityRemovalQueue.Count != 0)
            {
                foreach (uint entity in entityRemovalQueue)
                {
                    ClearEntity((int)entity);
                }
                entityRemovalQueue.Clear();
            }
        }

        protected void DrawECS()
        {
            systemManager.Draw(Camera);
        }
        #endregion

        #region Partitioning Support
        internal void FinalizePartition()
        {
            entitiesInView = Query(new Core.Rectangle(Camera.Bounds.X - queryBuffer, Camera.Bounds.Y - queryBuffer, Camera.Bounds.Width + queryBuffer * 2, Camera.Bounds.Height + queryBuffer * 2));
        }

        /// <summary>
        /// Returns whether or not an <see cref="CPartitionable"/> entity is currently within the camera's view and <see cref="PartitioningEnabled"/> is true.
        /// Otherwise, this will always return false.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool EntityIsVisible(int entity)
        {
            if (!PartitioningEnabled)
                return true;

            return entitiesInView.Contains((uint)entity);
        }

        /// <summary>
        /// Create and register a <see cref="SBin"/> system.
        /// </summary>
        /// <param name="maximumDimension">The maximum dimension of the entities expected.</param>
        protected void AddBinPartitioningSystem(int maximumDimension)
        {
            if (partitionerPreference != PartitionerType.None)
                return;

            PartitioningEnabled = true;
            partitionerPreference = PartitionerType.Bin;

            int optimalBinSize = (int)Math.Ceiling(Math.Log(maximumDimension, 2));
            bin = new Bin(SceneBounds, optimalBinSize, EntityCapacity);

            RegisterSystem(new SBin(this, bin));
        }

        protected void EnablePartitioning()
        {
            if (partitionerPreference == PartitionerType.None || PartitioningEnabled)
                return;

            PartitioningEnabled = true;
            GetSystem<SBin>().Enabled = true;
        }

        protected void DisablePartitioning()
        {
            if (partitionerPreference == PartitionerType.None || !PartitioningEnabled)
                return;

            PartitioningEnabled = false;
            GetSystem<SBin>().Enabled = false;
        }

        public SparseSet Query(Core.Rectangle bounds)
        {
            if (!PartitioningEnabled)
                return new SparseSet(0);

            return bin.Query(bounds);
        }

        public SparseSet Query(CPosition position, CDimension dimension, int buffer)
        {
            if (!PartitioningEnabled)
                return new SparseSet(0);

            return bin.Query(position, dimension, buffer);
        }

        #endregion

        protected void SetSceneBounds(int width, int height)
        {
            if (SceneBounds.Width == width && SceneBounds.Height == height)
                return;

            SceneBounds = new Core.Rectangle(0, 0, width, height);

            if (PartitioningEnabled)
            {
                bin.Boundary = SceneBounds;
            }

            Camera.SetMovementRestriction(0, 0, SceneBounds.Width, SceneBounds.Height);
        }

        public abstract void LoadScene();

        public abstract void UnloadScene();

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
