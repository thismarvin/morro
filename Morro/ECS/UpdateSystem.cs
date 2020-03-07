using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Morro.ECS
{
    /// <summary>
    /// An abstraction of a <see cref="MorroSystem"/> that can process or manipulate <see cref="IComponent"/> data on every frame or a fixed basis.
    /// Note that an <see cref="UpdateSystem"/> is always executed before a <see cref="DrawSystem"/>.
    /// </summary>
    abstract class UpdateSystem : MorroSystem
    {
        private readonly uint tasks;

        private bool fixedUpdateEnabled;
        private float target;
        private float accumulator;

        /// <summary>
        /// Create a <see cref="MorroSystem"/> that will process or manipulate <see cref="IComponent"/> data on every frame or a fixed basis.
        /// </summary>
        /// <param name="scene">The scene this system will exist in.</param>
        /// <param name="tasks">The total amount of tasks to divide the update cycle into. Assigning more than one task allows entities to be updated asynchronously.</param>
        public UpdateSystem(Scene scene, uint tasks) : base(scene)
        {
            this.tasks = tasks;
        }

        public void EnableFixedUpdate(int targetFPS)
        {
            fixedUpdateEnabled = true;
            target = 1f / targetFPS;
        }

        public virtual void Update()
        {
            if (fixedUpdateEnabled)
            {
                if (tasks <= 1)
                {
                    ExecuteAsFixedUpdate(NormalUpdate);
                }
                else
                {
                    ExecuteAsFixedUpdate(ParallelUpdate);
                }
            }
            else
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

            void ExecuteAsFixedUpdate(Action action)
            {
                accumulator += Engine.DeltaTime;
                while (accumulator >= target)
                {
                    action();
                    accumulator -= target;
                }
            }
        }

        /// <summary>
        /// The processing and manipulation logic executed on every entity associated with this system.
        /// </summary>
        /// <param name="entity">The current entity being processed by the system.</param>
        public abstract void UpdateEntity(int entity);

        private void NormalUpdate()
        {
            foreach (int entity in Entities)
            {
                UpdateEntity(entity);
            }
        }

        private void ParallelUpdate()
        {
            int[] entities = EntitiesAsArray;
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
                    for (int i = startingIndex; i < endingIndex; i++)
                    {
                        UpdateEntity(entities[i]);
                    }
                });
            }
        }

    }
}
