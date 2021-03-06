﻿using Example.Components;
using Example.Entities;
using Example.Systems;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Scenes
{
    class Flocking : Scene
    {
        public Flocking() : base("Flocking", 2000, 16, 16)
        {
            RegisterSystem
            (
                new SWrapAround(this),
                new SHunting(this),
                new SBinPartitioner(this, 64, 60),
                new SFlocking(this),
                new SPhysics(this),
                new STriangle(this)
            );
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
                CreateBoids();
            }
            if (Morro.Input.Mouse.PressedRightClick())
            {
                CreatePredators();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sketch.CreateBackgroundLayer(spriteBatch, PICO8.SkyBlue);

            Sketch.AttachEffect(new DropShadow(Engine.RenderTarget));
            Sketch.Begin(spriteBatch);
            {
                DrawECS();
            }
            Sketch.End(spriteBatch);
        }

        private void CreateBoids()
        {
            int buffer = 32;

            for (int i = 0; i < EntityCapacity * 0.1; i++)
            {
                CreateEntity(Boid.Create(Morro.Maths.Random.Range(buffer, (int)SceneBounds.Width - buffer), Morro.Maths.Random.Range(buffer, (int)SceneBounds.Height - buffer)));
            }
        }

        private void CreatePredators()
        {
            int buffer = 32;

            CreateEntity(Hawk.Create(Morro.Maths.Random.Range(buffer, (int)SceneBounds.Width - buffer), Morro.Maths.Random.Range(buffer, (int)SceneBounds.Height - buffer)));
        }
    }
}
