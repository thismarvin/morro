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
        public PartitionerType PartitionerPreference { get; private set; }
        public Partitioner Partitioner { get; private set; }
        public Camera Camera { get; private set; }
        public Transition EnterTransition { get; set; }
        public Transition ExitTransition { get; set; }
        public string Name { get; private set; }
        public Core.Rectangle SceneBounds { get; private set; }


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

        public Scene(string name, int maximumEntityCount = 100, int maximumComponentCount = 64, int maximumSystemCount = 64)
        {
            Name = SceneManager.FormatName(name);
            SceneBounds = new Core.Rectangle(0, 0, WindowManager.PixelWidth, WindowManager.PixelHeight);

            PartitionerPreference = PartitionerType.Quadtree;
            Partitioner = new Quadtree(SceneBounds, 4);

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
            foreach (uint i in attachedComponents[entity])
            {
                data[i][entity] = null;
            }

            foreach (uint i in attachedSystems[entity])
            {
                systems[i].RemoveEntity(entity);
            }

            attachedComponents[entity].Clear();
            attachedSystems[entity].Clear();
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

        public void UpdateECS()
        {
            for (int i = 0; i < systemIndex; i++)
            {
                systems[i].GrabData(this);
                systems[i].Update();
            }
        }

        public void DrawECS(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < systemIndex; i++)
            {
                systems[i].GrabData(this);
                systems[i].Draw(spriteBatch);
            }
        }

        #endregion



        //public List<Entity> Query(Core.Rectangle bounds)
        //{
        //    List<Entity> result = new List<Entity>();
        //    List<MonoObject> queryResult = Partitioner.Query(bounds);

        //    for (int i = 0; i < queryResult.Count; i++)
        //    {
        //        if (queryResult[i] is Entity)
        //        {
        //            result.Add((Entity)queryResult[i]);
        //        }
        //    }
        //    result.Sort();

        //    return result;
        //}

        /// <summary>
        /// Set the <see cref="PartitionerPreference"/> to <see cref="PartitionerType.Quadtree"/>, and initialize a new <see cref="Quadtree"/>.
        /// </summary>
        /// <param name="capacity">the amount of <see cref="MonoObject"/>'s allowed in each <see cref="Quadtree"/>.</param>
        protected void PreferQuadtreePartitioner(int capacity)
        {
            PartitionerPreference = PartitionerType.Quadtree;
            Partitioner = new Quadtree(SceneBounds, capacity);
        }

        /// <summary>
        /// Set the <see cref="PartitionerPreference"/> to <see cref="PartitionerType.Bin"/>, and initialize a new <see cref="Bin"/>.
        /// </summary>
        /// <param name="maximumDimension">the maximum dimension of the <see cref="MonoObject.Bounds"/> expected.</param>
        protected void PreferBinPartitioner(int maximumDimension)
        {
            PartitionerPreference = PartitionerType.Bin;
            int optimalBinSize = (int)Math.Ceiling(Math.Log(maximumDimension, 2));
            Partitioner = new Bin(SceneBounds, optimalBinSize);
        }

        protected void SetSceneBounds(int width, int height)
        {
            if (SceneBounds.Width == width && SceneBounds.Height == height)
                return;

            SceneBounds = new Core.Rectangle(0, 0, width, height);
            Partitioner.SetBoundary(SceneBounds);

            Camera.SetMovementRestriction(0, 0, SceneBounds.Width, SceneBounds.Height);
        }

        protected void SpatialPartitioning()
        {
            //Partitioner.Clear();
            //for (int i = 0; i < Entities.Count; i++)
            //{
            //    Partitioner.Insert(Entities[i]);
            //}
        }

        public abstract void LoadScene();

        public abstract void UnloadScene();

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
