using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using Morro.Input;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Morro.ECS
{
    class Flocking : Scene
    {
        private int totalBoids;

        public Flocking() : base(SceneType.Flocking)
        {

        }

        protected override void Initialize()
        {
            totalBoids = 750;
        }

        public override void LoadScene()
        {
            Entities.Clear();

            for (int i = 0; i < totalBoids; i++)
            {
                Entities.Add(new Boid(RandomHelper.Range(32, SceneBounds.Width - 32), RandomHelper.Range(32, SceneBounds.Height - 32)));
            }
        }

        public override void UnloadScene()
        {

        }

        public override void Update(GameTime gameTime)
        {
            UpdateEntities(gameTime, 4);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sketch.CreateBackgroundLayer(spriteBatch, PICO8.SkyBlue);

            Sketch.AttachEffect(new DropShadow(Engine.RenderTarget, new Vector2(1, 1), WindowManager.Scale));
            Sketch.Begin(spriteBatch);            
            {
                DrawEntities(spriteBatch);
            }
            Sketch.End(spriteBatch);            
        }
    }
}
