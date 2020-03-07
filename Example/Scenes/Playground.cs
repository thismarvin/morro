using Example.Components;
using Example.Entities;
using Example.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Debug;
using Morro.ECS;
using Morro.Graphics;
using Morro.Maths;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Example.Scenes
{
    class Playground : Scene
    {
        public Playground() : base("Playground", 50000, 8, 5)
        {
            RegisterSystem(new Cleaner(this));
            RegisterSystem(new PhysicsEngine(this));
            RegisterSystem(new QuadUpdater(this));
            RegisterSystem(new QuadRenderer(this));

            //AddBinPartitioningSystem(64);

            Camera.SmoothTrackingSpeed = 5;
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

            if (Morro.Input.Mouse.PressedLeftClick())
            {
                for (int i = 0; i < EntityCapacity * 0.25; i++)
                {
                    CreateEntity(Yoman.Create(Morro.Input.Mouse.SceneLocation.X, Morro.Input.Mouse.SceneLocation.Y, Morro.Maths.Random.Range(1, 5)));
                }
            }

            if (Morro.Input.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                GetSystem<PhysicsEngine>().Enabled = !GetSystem<PhysicsEngine>().Enabled;
            }

            if (Morro.Input.Keyboard.Pressing(Microsoft.Xna.Framework.Input.Keys.D))
            {
                Camera.SmoothTrack(Camera.Center.X + 50, Camera.Center.Y);
            }

            if (Morro.Input.Keyboard.Pressing(Microsoft.Xna.Framework.Input.Keys.A))
            {
                Camera.SmoothTrack(Camera.Center.X - 50, Camera.Center.Y);
            }

            if (Morro.Input.Keyboard.Pressing(Microsoft.Xna.Framework.Input.Keys.W))
            {
                Camera.SmoothTrack(Camera.Center.X, Camera.Center.Y - 50);
            }

            if (Morro.Input.Keyboard.Pressing(Microsoft.Xna.Framework.Input.Keys.S))
            {
                Camera.SmoothTrack(Camera.Center.X, Camera.Center.Y + 50);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sketch.CreateBackgroundLayer(spriteBatch, Color.CornflowerBlue);

            Sketch.AttachEffect(new ChromaticAberration(Engine.RenderTarget, 2));
            Sketch.Begin(spriteBatch);
            {
                DrawECS();
            }
            Sketch.End(spriteBatch);
        }
    }
}
