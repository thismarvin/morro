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

        public int SystemCapacity { get => systemManager.Capacity; }
        public int ComponentCapacity { get => componentManager.Capacity; }
        public int EntityCapacity { get => entityManager.Capacity; }

        private readonly SystemManager systemManager;
        private readonly ComponentManager componentManager;
        private readonly EntityManager entityManager;

        private readonly SparseSet entityRemovalQueue;
        private SparseSet entitiesInView;

        private readonly int queryBuffer;

        public Scene(string name, int entityCapacity = 100, int componentCapacity = 64, int systemCapacity = 64)
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
            int entity = entityManager.AllocateEntity(ComponentCapacity, SystemCapacity);
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
            SpatialPartitioning();

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

        public SparseSet Query(Core.Rectangle bounds)
        {
            SparseSet result = new SparseSet(EntityCapacity);

            if (!PartitioningEnabled)
            {
                for (int entity = 0; entity < EntityCapacity; entity++)
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

            return entitiesInView.Contains((uint)entity);
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
            for (int entity = 0; entity < EntityCapacity; entity++)
            {
                if (EntityContains(entity, typeof(CPosition), typeof(CDimension)))
                {
                    position = GetData<CPosition>(entity);
                    dimension = GetData<CDimension>(entity);

                    Partitioner.Insert(new PartitionEntry(entity, new Core.Rectangle(position.X, position.Y, (int)dimension.Width, (int)dimension.Height)));
                }
                else if (EntityContains(entity, typeof(CPosition)))
                {
                    position = GetData<CPosition>(entity);

                    Partitioner.Insert(new PartitionEntry(entity, new Core.Rectangle(position.X, position.Y, 1, 1)));
                }
            }

            entitiesInView = Query(new Core.Rectangle(Camera.Bounds.X - queryBuffer, Camera.Bounds.Y - queryBuffer, Camera.Bounds.Width + queryBuffer * 2, Camera.Bounds.Height + queryBuffer * 2));
        }

        public abstract void LoadScene();

        public abstract void UnloadScene();

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
