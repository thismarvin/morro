using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class SWrapAround : UpdateSystem
    {
        private IComponent[] positions;
        private IComponent[] dimensions;

        public SWrapAround(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CDimension));
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];

            if (position.X + dimension.Width < scene.SceneBounds.Left)
            {
                position.X = scene.SceneBounds.Right;
            }
            else if (position.X > scene.SceneBounds.Right)
            {
                position.X = scene.SceneBounds.Left - dimension.Width;
            }
            if (position.Y + dimension.Height < scene.SceneBounds.Top)
            {
                position.Y = scene.SceneBounds.Bottom;
            }
            else if (position.Y > scene.SceneBounds.Bottom)
            {
                position.Y = scene.SceneBounds.Top - dimension.Height;
            }
        }

        public override void Update()
        {
            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();

            base.Update();
        }
    }
}
