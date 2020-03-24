using Example.Entities;
using Example.Systems;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using Morro.Graphics.Effects;
using Morro.Graphics.Palettes;
using Morro.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Scenes
{
    class Flocking : Scene
    {
        public Flocking() : base("Flocking", 2000, 16, 6)
        {
            RegisterSystems
            (
                new SWrapAround(this),
                new SHunting(this),
                new SBinPartitioner(this, 64, 60),
                new SFlocking(this),
                new SBoidPhysics(this),
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
            RunUpdateableSystems();

            if (MoreMouse.PressedLeftClick())
            {
                CreateBoids();
            }
            if (MoreMouse.PressedRightClick())
            {
                CreatePredators();
            }
        }

        public override void Draw()
        {
            Sketch.CreateBackgroundLayer(PICO8.SkyBlue);

            Sketch.AttachEffect(new DropShadow(Engine.RenderTarget));
            Sketch.Begin();
            {
                RunDrawableSystems();
            }
            Sketch.End();
        }

        private void CreateBoids()
        {
            int buffer = 32;

            for (int i = 0; i < EntityCapacity * 0.1; i++)
            {
                CreateEntity(Boid.Create(Morro.Maths.MoreRandom.Range(buffer, (int)SceneBounds.Width - buffer), Morro.Maths.MoreRandom.Range(buffer, (int)SceneBounds.Height - buffer)));
            }
        }

        private void CreatePredators()
        {
            int buffer = 32;

            CreateEntity(Hawk.Create(Morro.Maths.MoreRandom.Range(buffer, (int)SceneBounds.Width - buffer), Morro.Maths.MoreRandom.Range(buffer, (int)SceneBounds.Height - buffer)));
        }
    }
}
