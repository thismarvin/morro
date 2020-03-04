using Microsoft.Xna.Framework.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Morro.ECS
{
    abstract class UpdateSystem : MorroSystem
    {
        private readonly uint tasks;

        public UpdateSystem(Scene scene, uint tasks) : base(scene)
        {
            this.tasks = tasks;
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
                        UpdateEntity(EntitiesAsArray[entity]);
                    }
                });
            }
        }

        public abstract void UpdateEntity(int entity);

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
    }
}
