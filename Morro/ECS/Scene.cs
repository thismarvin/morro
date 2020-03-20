using Morro.Core;
using Morro.Graphics;
using Morro.Graphics.Transitions;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    abstract class Scene
    {
        /// <summary>
        /// By default all registered <see cref="MorroSystem"/>'s are ran asyncronously.
        /// If this functionality is disabled, systems will run in the order they were registered.
        /// </summary>
        public bool AsynchronousSystemsEnabled
        {
            get => !systemManager.DisableAsynchronousUpdates;
            set => systemManager.DisableAsynchronousUpdates = !value;
        }

        public Camera Camera { get; private set; }
        public Transition EnterTransition { get; set; }
        public Transition ExitTransition { get; set; }
        public string Name { get; private set; }
        public Core.RectangleF SceneBounds { get; private set; }

        public int SystemCapacity { get => systemManager.Capacity; }
        public int ComponentCapacity { get => componentManager.Capacity; }
        public int EntityCapacity { get => entityManager.Capacity; }
        public int EntityCount { get => entityManager.EntityCount; }

        private readonly SystemManager systemManager;
        private readonly ComponentManager componentManager;
        private readonly EntityManager entityManager;
        private readonly EventManager eventManager;

        private readonly SparseSet entityRemovalQueue;

        public Scene(string name, int entityCapacity = 100, int componentCapacity = 64, int systemCapacity = 64)
        {
            Name = name;
            SceneBounds = new RectangleF(0, 0, WindowManager.PixelWidth, WindowManager.PixelHeight);

            Camera = new Camera(Name);
            Camera.SetMovementRestriction(0, 0, SceneBounds.Width, SceneBounds.Height);
            CameraManager.RegisterCamera(Camera);

            EnterTransition = new Pinhole(TransitionType.Enter);
            ExitTransition = new Fade(TransitionType.Exit);

            systemManager = new SystemManager(systemCapacity);
            componentManager = new ComponentManager(componentCapacity, entityCapacity);
            entityManager = new EntityManager(entityCapacity, systemManager, componentManager);
            eventManager = new EventManager(systemManager);

            entityRemovalQueue = new SparseSet(EntityCapacity);
        }

        #region ECS Stuff
        public void RegisterSystem(params MorroSystem[] morroSystem)
        {
            systemManager.RegisterSystem(morroSystem);
            eventManager.Crawl();
        }

        public int CreateEntity(params IComponent[] components)
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

        public void AddComponent(int entity, params IComponent[] components)
        {
            entityManager.AddComponent(entity, components);
        }

        public void RemoveComponent(int entity, params Type[] componentTypes)
        {
            entityManager.RemoveComponent(entity, componentTypes);
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

        private void ClearEntity(int entity)
        {
            entityManager.ClearEntity(entity);
        }
        #endregion

        protected void SetSceneBounds(int width, int height)
        {
            if (SceneBounds.Width == width && SceneBounds.Height == height)
                return;

            SceneBounds = new RectangleF(0, 0, width, height);

            Camera.SetMovementRestriction(0, 0, SceneBounds.Width, SceneBounds.Height);
        }

        public abstract void LoadScene();

        public abstract void UnloadScene();

        public abstract void Update();

        public abstract void Draw();
    }
}
