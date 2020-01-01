using Example.Components;
using Example.Entities;
using Example.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Scenes
{
    class Playground : Scene
    {
        public Playground() : base("Playground")
        {          
            RegisterSystem(new PhysicsSystem());
            RegisterSystem(new SBoxRenderer());

            for (int i = 0; i < maximumEntityCount; i++)
            {
                CreateEntity(Yoman.Create(16, 16));
            }
        }

        public override void LoadScene()
        {

        }

        public override void UnloadScene()
        {

        }

        public override void Update()
        {
            UpdateECS();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sketch.CreateBackgroundLayer(spriteBatch, Color.Blue);

            Sketch.Begin(spriteBatch);
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, GraphicsManager.DefaultRasterizerState, null, Camera.Transform);
                DrawECS(spriteBatch);
                spriteBatch.End();
            }            
            Sketch.End(spriteBatch);
        }
    }
}
