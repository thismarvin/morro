﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Example.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;

namespace Example.Systems
{
    class SPhysicsSystem : MorroSystem
    {
        private IComponent[] transforms;
        private IComponent[] physicsBodies;

        //private double accumulator;
        private readonly float target;
        private readonly Integrator integrator;

        private enum Integrator
        {
            SemiImplictEuler,
            VelocityVerlet
        }

        public SPhysicsSystem(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CPhysicsBody));

            target = 1f / 120;
            integrator = Integrator.SemiImplictEuler;
        }

        public override void GrabData(Scene scene)
        {
            transforms = scene.GetData<CPosition>();
            physicsBodies = scene.GetData<CPhysicsBody>();
        }

        public override void UpdateEntity(int entity)
        {
            //Integrate(entity, integrator, target);

            if (!scene.EntitiesInView.Contains((uint)entity))
                return;

            CPhysicsBody physicsBody = (CPhysicsBody)physicsBodies[entity];

            physicsBody.Accumulator += (float)(Engine.TotalGameTime - physicsBody.LastUpdate).TotalSeconds;
            physicsBody.LastUpdate = new TimeSpan(Engine.TotalGameTime.Ticks);

            while (physicsBody.Accumulator >= target)
            {
                Integrate(entity, integrator, target);
                physicsBody.Accumulator -= target;
            }
        }

        public override void DrawEntity(int entity, SpriteBatch spriteBatch)
        {

        }

        //public override void Update()
        //{
        //    Simultate(Engine.DeltaTime);
        //}

        //private void Simultate(float elapsedTime)
        //{
        //    accumulator += elapsedTime;

        //    while (accumulator >= target)
        //    {
        //        ParallelUpdate();
        //        accumulator -= target;
        //    }
        //}

        private void Integrate(int entity, Integrator integrator, float deltaTime)
        {
            CPosition position = (CPosition)transforms[entity];
            CPhysicsBody physicsBody = (CPhysicsBody)physicsBodies[entity];

            switch (integrator)
            {
                case Integrator.SemiImplictEuler:
                    SemiImplictEulerIntegration(deltaTime, position, physicsBody);
                    break;

                case Integrator.VelocityVerlet:
                    VelocityVerletIntegration(deltaTime, position, physicsBody);
                    break;
            }
        }

        private void SemiImplictEulerIntegration(float deltaTime, CPosition position, CPhysicsBody physicsBody)
        {
            physicsBody.SetVelocity
            (
                physicsBody.Velocity.X + physicsBody.Acceleration.X * deltaTime,
                physicsBody.Velocity.Y + physicsBody.Acceleration.Y * deltaTime
            );

            //position.SetLocation
            //(
            //    position.X + physicsBody.Velocity.X * deltaTime,
            //    position.Y + physicsBody.Velocity.Y * deltaTime
            //);

            position.X += physicsBody.Velocity.X * deltaTime;
            position.Y += physicsBody.Velocity.Y * deltaTime;
        }

        private static void VelocityVerletIntegration(float deltaTime, CPosition position, CPhysicsBody physicsBody)
        {
            //position.SetLocation
            //(
            //    position.X + physicsBody.Velocity.X * deltaTime + 0.5f * physicsBody.Acceleration.X * deltaTime * deltaTime,
            //    position.Y + physicsBody.Velocity.Y * deltaTime + 0.5f * physicsBody.Acceleration.Y * deltaTime * deltaTime
            //);

            position.X += physicsBody.Velocity.X * deltaTime + 0.5f * physicsBody.Acceleration.X * deltaTime * deltaTime;
            position.Y += physicsBody.Velocity.Y * deltaTime + 0.5f * physicsBody.Acceleration.Y * deltaTime * deltaTime;

            physicsBody.SetVelocity
            (
                physicsBody.Velocity.X + physicsBody.Acceleration.X * deltaTime,
                physicsBody.Velocity.Y + physicsBody.Acceleration.Y * deltaTime
            );
        }
    }
}