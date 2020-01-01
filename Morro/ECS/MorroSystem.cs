using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Morro.ECS
{
    abstract class MorroSystem
    {
        public HashSet<Type> RequiredComponents { get; private set; }
        public HashSet<int> Entities { get; private set; }

        protected List<int> entitiesAsList;
        protected uint tasks;

        public MorroSystem(uint tasks = 1)
        {
            RequiredComponents = new HashSet<Type>();
            Entities = new HashSet<int>();
            entitiesAsList = new List<int>();
            this.tasks = tasks;
        }

        public void Require(params Type[] components)
        {
            RequiredComponents.Clear();

            for (int i = 0; i < components.Length; i++)
            {
                RequiredComponents.Add(components[i]);
            }
        }

        internal void AddEntity(int entity)
        {
            if (Entities.Contains(entity))
                return;

            Entities.Add(entity);
            entitiesAsList.Add(entity);
        }

        internal void RemoveEntity(int entity)
        {
            Entities.Remove(entity);
            entitiesAsList.Remove(entity);
        }

        private void NormalUpdate()
        {
            foreach (int entity in Entities)
            {
                UpdateEntity(entity);
            }
        }

        private void ParallelUpdate()
        {
            Task.WaitAll(DivideUpdateIntoTasks(tasks));

            Task[] DivideUpdateIntoTasks(uint totalTasks)
            {
                Task[] result = new Task[totalTasks];

                int increment = Entities.Count / (int)totalTasks;
                int start = 0;
                int end = increment;

                for (int i = 0; i < totalTasks; i++)
                {
                    if (i != totalTasks - 1)
                    {
                        result[i] = UpdateSection(start, end);
                    }
                    else
                    {
                        result[i] = UpdateSection(start, Entities.Count);
                    }

                    start += increment;
                    end += increment;
                }

                return result;
            }

            Task UpdateSection(int startingIndex, int endingIndex)
            {
                return Task.Run(() =>
                {
                    for (int entity = startingIndex; entity < endingIndex; entity++)
                    {
                        UpdateEntity(entitiesAsList[entity]);
                    }
                });
            }
        }

        public abstract void GrabData(Scene scene);
        public abstract void UpdateEntity(int entity);
        public abstract void DrawEntity(int entity, SpriteBatch spriteBatch);

        public virtual void Update()
        {
            if (tasks <= 1)
            {
                NormalUpdate();
            }
            else
            {
                ParallelUpdate();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (int entity in Entities)
            {
                DrawEntity(entity, spriteBatch);
            }
        }
    }
}
