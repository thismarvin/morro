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
            RegisterSystem(new SPhysicsSystem(this));
            RegisterSystem(new SBoxHandler(this));

            //for (int i = 0; i < maximumEntityCount; i++)
            //{
            //    CreateEntity(Yoman.Create(SceneBounds.Width / 2, SceneBounds.Height / 2, RandomHelper.Range(1, 5)));
            //}

            PreferBinPartitioner(8);

            SetSceneBounds(2000, 500);
        }

        public override void LoadScene()
        {

        }

        public override void UnloadScene()
        {

        }

        public override void Update()
        {
            if (Morro.Input.Mouse.PressingLeftClick())
            {
                //for (int i = 0; i < maximumEntityCount; i++)
                //{
                //    CreateEntity(Yoman.Create(SceneBounds.Width / 2, SceneBounds.Height / 2, RandomHelper.Range(1, 5)));
                //}

                CreateEntity(Yoman.Create(Morro.Input.Mouse.SceneLocation.X, Morro.Input.Mouse.SceneLocation.Y, RandomHelper.Range(1, 5)));
            }

            UpdateECS();

            if (Morro.Input.Keyboard.Pressing(Microsoft.Xna.Framework.Input.Keys.D))
            {
                Camera.Track( 1 * Engine.DeltaTime, Camera.TrackingPosition.Y);
            }

            Console.WriteLine(Camera.Bounds);

            //SpatialPartitioning();
            Console.WriteLine(Query(Camera.Bounds).Count);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sketch.CreateBackgroundLayer(spriteBatch, Color.Blue);

            Sketch.AttachEffect(new Palette());
            Sketch.Begin(spriteBatch);
            {
                //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, GraphicsManager.DefaultRasterizerState, null, Camera.Transform);
                DrawECS(spriteBatch);
                //spriteBatch.End();
            }            
            Sketch.End(spriteBatch);
        }
    }
}
