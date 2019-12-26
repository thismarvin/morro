using Example.Entities;
using Example.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.ECS;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Scenes
{
    class Playground : Scene
    {
        public Playground() : base("Playground")
        {          
            AddSystem(new Drawer());
            AddSystem(new PhysicsSystem());

            for (int i = 0; i < 999; i++)
            {
                Yoman.Create(this, 16, 16);
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
                DrawECS(spriteBatch);
            }            
            Sketch.End(spriteBatch);
        }
    }
}
