using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class SRemove : UpdateSystem
    {
        private IComponent[] positions;

        public SRemove(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition));
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];

            if (position.Y > scene.SceneBounds.Bottom + 64 || position.X < scene.SceneBounds.Left - 64 || position.X > scene.SceneBounds.Right + 64)
            {
                scene.RemoveEntity(entity);
            }
        }

        public override void Update()
        {
            positions = scene.GetData<CPosition>();

            base.Update();
        }
    }
}
