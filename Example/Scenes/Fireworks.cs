using Example.Entities;
using Example.Systems;
using Microsoft.Xna.Framework;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using Morro.Graphics.Effects;
using Morro.Input;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Scenes
{
    class Fireworks : Scene
    {
        public Fireworks() : base("Fireworks", 50000, 8, 3)
        {
            RegisterSystem
            (
                new SRemove(this),
                new SPhysics(this, Integrator.SemiImplictEuler, 4, 120),
                new SQuad(this)
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

            if (MoreMouse.PressedLeftClick())
            {
                for (int i = 0; i < EntityCapacity * 0.25; i++)
                {
                    CreateEntity(Particle.Create(MoreMouse.SceneLocation.X, MoreMouse.SceneLocation.Y, MoreRandom.Range(1, 5)));
                }
            }
        }

        public override void Draw()
        {
            Sketch.CreateBackgroundLayer(Color.CornflowerBlue);

            Sketch.AttachEffect(new ChromaticAberration(Engine.RenderTarget, 2));
            Sketch.Begin();
            {
                DrawECS();
            }
            Sketch.End();
        }
    }
}
