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
using System.Diagnostics;
using System.Text;

namespace Example.Scenes
{
    class Playground : Scene
    {
        readonly MPolygon polygon;
        readonly MLine test;

        public Playground() : base("Playground", 100, 8, 4)
        {
            RegisterSystem(new SPhysicsSystem(this));
            RegisterSystem(new SBoxHandler(this));

            Camera.SmoothTrackingSpeed = 5;

            polygon = new MAABB(16, 16, 32, 32)
            {
                LineWidth = 8,
                Rotation = MathHelper.PiOver4,
                RotationOffset = new Vector3(16, 16, 0),
                Color = Color.Red
            };

            test = new MLine
            (
                new Vector2[]
                {
                    new Vector2(16 * 1, 16 * 1),
                    new Vector2(16 * 2, 16 * 3),
                    new Vector2(16 * 3, 16 * 1),
                    new Vector2(16 * 4, 16 * 2),
                    new Vector2(16 * 5, 16 * 2),
                    new Vector2(16 * 6, 16 * 4),
                    new Vector2(16 * 6, 16 * 8),
                }
            )
            {
                LineWidth = 4
            };

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
                    CreateEntity(Yoman.Create(Morro.Input.Mouse.SceneLocation.X, Morro.Input.Mouse.SceneLocation.Y, Morro.Maths.Random.Range(1, 5)));
                }
            }

            UpdateECS();

            polygon.X = Morro.Input.Mouse.SceneLocation.X;
            polygon.Y = Morro.Input.Mouse.SceneLocation.Y;

            //test.SetEndPoint(polygon.X, polygon.Y);

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

            Sketch.Begin(spriteBatch);
            {
                DrawECS(spriteBatch);

                polygon.Draw(spriteBatch, Camera);

                test.Draw(spriteBatch, Camera);
            }
            Sketch.End(spriteBatch);
        }
    }
}
