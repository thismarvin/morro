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
        public List<Entity> Entities { get; private set; }
        public List<Entity> EntityBuffer { get; private set; }
        public PartitionerType PartitionerPreference { get; private set; }
        public Partitioner Partitioner { get; private set; }
        public Camera Camera { get; private set; }
        public Transition EnterTransition { get; set; }
        public Transition ExitTransition { get; set; }
        public string Name { get; private set; }
        public Core.Rectangle SceneBounds { get; private set; }


        private int totalEntities;
        private int nextEntity;

        private readonly Dictionary<string, Component[]> data;
        private readonly Dictionary<string, System> systems;
        public ComponentBag[] Lentities { get; private set; }

        public Scene(string name)
        {
            Entities = new List<Entity>();
            EntityBuffer = new List<Entity>();

            Name = SceneManager.FormatName(name);
            SceneBounds = new Core.Rectangle(0, 0, WindowManager.PixelWidth, WindowManager.PixelHeight);

            PartitionerPreference = PartitionerType.Quadtree;
            Partitioner = new Quadtree(SceneBounds, 4);

            Camera = new Camera(Name);
            Camera.SetMovementRestriction(0, 0, SceneBounds.Width, SceneBounds.Height);
            CameraManager.RegisterCamera(Camera);

            EnterTransition = new Pinhole(TransitionType.Enter);
            ExitTransition = new Fade(TransitionType.Exit);

            totalEntities = 1000;
            data = new Dictionary<string, Component[]>();
            systems = new Dictionary<string, System>();
            Lentities = new ComponentBag[totalEntities];
        }

        #region ECS Stuff

        public void AddSystem(System system)
        {
            if (systems.ContainsKey(system.Name))
                return;

            systems.Add(system.Name, system);
        }

        public int AllocateEntity()
        {
            int next = nextEntity;
            Lentities[next] = new ComponentBag();

            nextEntity = nextEntity + 1 > totalEntities ? 0 : ++nextEntity;
            
            return next;
        }

        public void Require(string componentName)
        {
            string formattedName = ComponentManager.FormatName(componentName);

            if (data.ContainsKey(formattedName))
                return;

            data.Add(formattedName, new Component[totalEntities]);
        }

        public void RegisterEntity(int entity)
        {
            foreach (KeyValuePair<string, System> entry in systems)
            {
                entry.Value.RegisterEntity(this, entity);
            }
        }

        public Component[] GetData(string componentName)
        {
            string formattedName = ComponentManager.FormatName(componentName);

            if (!data.ContainsKey(formattedName))
                return new Component[0];

            return data[formattedName];
        }

        public void UpdateECS()
        {
            foreach (KeyValuePair<string, System> entry in systems)
            {
                entry.Value.Update(this);
            }

        }

        public void DrawECS(SpriteBatch spriteBatch)
        {
            foreach (KeyValuePair<string, System> entry in systems)
            {
                entry.Value.Draw(spriteBatch, this);
            }
        }

        public void AssignComponent(int entity, Component data)
        {
            if (!this.data.ContainsKey(data.Name))
                throw new MorroException("didnt require that component type", new KeyNotFoundException());

            Lentities[entity].AddComponent(data.Name);
            this.data[data.Name][entity] = data;
        }

        public Component GetComponent(string componentName, int entity)
        {
            string formattedName = ComponentManager.FormatName(componentName);

            return data[formattedName][entity];
        }
        #endregion
        public List<Entity> Query(Core.Rectangle bounds)
        {
            List<Entity> result = new List<Entity>();
            List<MonoObject> queryResult = Partitioner.Query(bounds);

            for (int i = 0; i < queryResult.Count; i++)
            {
                if (queryResult[i] is Entity)
                {
                    result.Add((Entity)queryResult[i]);
                }
            }
            result.Sort();

            return result;
        }

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
            Partitioner.Clear();
            for (int i = 0; i < Entities.Count; i++)
            {
                Partitioner.Insert(Entities[i]);
            }
        }

        protected virtual void UpdateEntities()
        {
            SpatialPartitioning();

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entities[i].Update();

                if (Entities[i].Remove)
                    Entities.RemoveAt(i);
            }

            if (EntityBuffer.Count > 0)
            {
                for (int i = 0; i < EntityBuffer.Count; i++)
                    Entities.Add(EntityBuffer[i]);

                EntityBuffer.Clear();
            }
        }

        private void UpdateSections(int start, int end)
        {
            for (int i = end - 1; i >= start; i--)
            {
                Entities[i].Update();

                if (Entities[i].Remove)
                    Entities.RemoveAt(i);
            }
        }

        private Task UpdateSection(int start, int end)
        {
            return Task.Run(() => UpdateSections(start, end));
        }

        private Task[] DivideUpdateIntoTasks(int totalTasks)
        {
            Task[] result = new Task[totalTasks];
            int increment = Entities.Count / totalTasks;
            int start = 0;
            int end = increment;
            for (int i = 0; i < totalTasks; i++)
            {
                if (i != totalTasks - 1)
                    result[i] = UpdateSection(start, end);
                else
                    result[i] = UpdateSection(start, Entities.Count);

                start += increment;
                end += increment;
            }
            return result;
        }

        protected virtual void UpdateEntities(int sections)
        {
            SpatialPartitioning();

            Task.WaitAll(
                DivideUpdateIntoTasks(sections)
            );
        }

        protected virtual void DrawEntities(SpriteBatch spriteBatch)
        {
            List<Entity> queryResult = Query(Camera.Bounds);
            for (int i = 0; i < queryResult.Count; i++)
            {
                queryResult[i].Draw(spriteBatch, Camera);
            }
        }

        public abstract void LoadScene();

        public abstract void UnloadScene();

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
