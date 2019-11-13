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
        public Transition EnterTransition { get; set; }
        public Transition ExitTransition { get; set; }
        public SceneType SceneType { get; private set; }
        public Core.Rectangle SceneBounds { get; protected set; }

        public Scene(SceneType type)
        {
            Entities = new List<Entity>();
            EntityBuffer = new List<Entity>();

            SceneType = type;
            SceneBounds = new Core.Rectangle(0, 0, WindowManager.PixelWidth, WindowManager.PixelHeight);

            PartitionerPreference = PartitionerType.Quadtree;
            Partitioner = new Quadtree(SceneBounds, 4);

            EnterTransition = new Pinhole(TransitionType.Enter);
            ExitTransition = new Fade(TransitionType.Exit);

            Initialize();
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

        protected void SpatialPartitioning()
        {
            Partitioner.Clear();
            for (int i = 0; i < Entities.Count; i++)
            {
                Partitioner.Insert(Entities[i]);
            }
        }

        protected virtual void UpdateEntities(GameTime gameTime)
        {
            SpatialPartitioning();

            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entities[i].Update(gameTime);

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

        protected void UpdateSections(GameTime gameTime, int start, int end)
        {
            for (int i = end - 1; i >= start; i--)
            {
                Entities[i].Update(gameTime);

                if (Entities[i].Remove)
                    Entities.RemoveAt(i);
            }
        }

        private Task UpdateSection(GameTime gameTime, int start, int end)
        {
            return Task.Run(() => UpdateSections(gameTime, start, end));
        }

        private Task[] DivideUpdateIntoTasks(GameTime gameTime, int totalTasks)
        {
            Task[] result = new Task[totalTasks];
            int increment = Entities.Count / totalTasks;
            int start = 0;
            int end = increment;
            for (int i = 0; i < totalTasks; i++)
            {
                if (i != totalTasks - 1)
                    result[i] = UpdateSection(gameTime, start, end);
                else
                    result[i] = UpdateSection(gameTime, start, Entities.Count);

                start += increment;
                end += increment;
            }
            return result;
        }

        protected virtual void UpdateEntities(GameTime gameTime, int sections)
        {
            SpatialPartitioning();

            Task.WaitAll(
                DivideUpdateIntoTasks(gameTime, sections)
            );
        }

        protected virtual void DrawEntities(SpriteBatch spriteBatch)
        {
            List<MonoObject> queryResult = Partitioner.Query(CameraManager.GetCamera(CameraType.Dynamic).Bounds);
            for (int i = 0; i < queryResult.Count; i++)
            {
                if (queryResult[i] is Entity)
                {
                    ((Entity)queryResult[i]).Draw(spriteBatch);
                }
            }
        }

        protected abstract void Initialize();

        public abstract void LoadScene();

        public abstract void UnloadScene();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
