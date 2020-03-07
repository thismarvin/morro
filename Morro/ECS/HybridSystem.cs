using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    abstract class HybridSystem : MorroSystem, IUpdateableSystem, IDrawableSystem
    {
        private readonly UpdateSystemHandler updateSystemHandler;
        private readonly DrawSystemHandler drawSystemHandler;

        public HybridSystem(Scene scene, uint tasks) : this(scene, tasks, 0)
        {

        }

        public HybridSystem(Scene scene, uint tasks, int targetFPS) : base(scene)
        {
            updateSystemHandler = new UpdateSystemHandler(this, UpdateEntity)
            {
                TotalTasks = tasks,
                TargetFPS = targetFPS
            };

            drawSystemHandler = new DrawSystemHandler(this, DrawEntity);
        }

        public virtual void Update()
        {
            updateSystemHandler.Update();
        }

        public virtual void Draw(Camera camera)
        {
            drawSystemHandler.Draw(camera);
        }

        public abstract void UpdateEntity(int entity);

        public abstract void DrawEntity(int entity, Camera camera);
    }
}
