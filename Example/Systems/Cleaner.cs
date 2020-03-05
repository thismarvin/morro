using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class Cleaner : UpdateSystem
    {
        private IComponent[] positions;

        public Cleaner(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition));
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];

            if (position.Y > scene.SceneBounds.Height + 64)
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
