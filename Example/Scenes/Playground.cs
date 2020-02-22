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
        PolygonBuilder polygonBuilder;

        public Playground() : base("Playground", 10000, 8, 4)
        {
            RegisterSystem(new SPhysicsSystem(this));
            RegisterSystem(new SBoxHandler(this));

            Camera.SmoothTrackingSpeed = 5;

            polygonBuilder = new PolygonBuilder();

            DisablePartitioning();
        }

        public override void LoadScene()
        {

        }

        public override void UnloadScene()
        {

        }

        public override void Update()
        {
            if (Morro.Input.Mouse.PressedLeftClick())
            {
                for (int i = 0; i < maximumEntityCount * 0.25; i++)
                {
                    CreateEntity(Yoman.Create(Morro.Input.Mouse.SceneLocation.X, Morro.Input.Mouse.SceneLocation.Y, RandomHelper.Range(1, 5)));
                }

                //polygonBuilder.AddVertex(Morro.Input.Mouse.SceneLocation.X, Morro.Input.Mouse.SceneLocation.Y);

            }            


            UpdateECS();

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
                Camera.SmoothTrack(Camera.Center.X, Camera.Center.Y -50);
            }

            if (Morro.Input.Keyboard.Pressing(Microsoft.Xna.Framework.Input.Keys.S))
            {
                Camera.SmoothTrack(Camera.Center.X, Camera.Center.Y + 50);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sketch.CreateBackgroundLayer(spriteBatch, Color.CornflowerBlue);

            Sketch.Begin(spriteBatch);
            {
                DrawECS(spriteBatch);
                //polygonBuilder.Draw(spriteBatch, Camera);
            }
            Sketch.End(spriteBatch);
        }
    }
}
