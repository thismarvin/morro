using Example.Components;
using Microsoft.Xna.Framework.Graphics;
using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class SBoxHandler : MorroSystem
    {
        private IComponent[] cPositions;
        private IComponent[] cBoxes;

        public SBoxHandler(Scene scene) : base(scene)
        {
            Require(typeof(CPosition), typeof(CBox));
        }

        public override void GrabData(Scene scene)
        {
            cPositions = scene.GetData<CPosition>();
            cBoxes = scene.GetData<CBox>();
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)cPositions[entity];
            CBox box = (CBox)cBoxes[entity];

            box.AABB.SetPosition(position.X, position.Y);
        }

        public override void DrawEntity(int entity, SpriteBatch spriteBatch)
        {
            CBox box = (CBox)cBoxes[entity];

            box.AABB.Draw(spriteBatch, scene.Camera);
        }
    }
}
