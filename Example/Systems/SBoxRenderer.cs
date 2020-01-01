using Example.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class SBoxRenderer : MorroSystem
    {
        private IComponent[] transforms;

        public SBoxRenderer()
        {
            Require(typeof(Transform));
        }

        public override void GrabData(Scene scene)
        {
            transforms = scene.GetData<Transform>();
        }

        public override void UpdateEntity(int entity)
        {

        }

        public override void DrawEntity(int entity, SpriteBatch spriteBatch)
        {
            Transform transform = (Transform)transforms[entity];
            spriteBatch.Draw(GraphicsManager.SimpleTexture, new Microsoft.Xna.Framework.Rectangle((int)transform.X, (int)transform.Y, 16, 16), Color.Black);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Transform transform;
            foreach (int entity in Entities)
            {
                transform = (Transform)transforms[entity];
                spriteBatch.Draw(GraphicsManager.SimpleTexture, new Microsoft.Xna.Framework.Rectangle((int)transform.X, (int)transform.Y, 16, 16), Color.Black);
            }
        }
    }
}
