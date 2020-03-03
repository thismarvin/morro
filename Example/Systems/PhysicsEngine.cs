﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Example.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;
using Morro.Utilities;

namespace Example.Systems
{
    class PhysicsEngine : UpdateSystem
    {
        private readonly float target;
        private readonly Integrator integrator;

        private enum Integrator
        {
            SemiImplictEuler,
            VelocityVerlet
        }

        public PhysicsEngine(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CPhysicsBody));

            target = 1f / 120;
            integrator = Integrator.SemiImplictEuler;
        }

        public override void UpdateEntity(int entity)
        {
            if (!scene.EntityInView(entity))
                return;

            CPosition position = scene.GetData<CPosition>(entity);

            if (position.Y > scene.SceneBounds.Height + 16)
            {
                scene.RemoveEntity(entity);
                return;
            }

            Simultate(entity);
        }

        private void Simultate(int entity)
        {
            CPhysicsBody physicsBody = scene.GetData<CPhysicsBody>(entity);

            physicsBody.Accumulator += (float)(Engine.TotalGameTime - physicsBody.LastUpdate).TotalSeconds;
            physicsBody.LastUpdate = new TimeSpan(Engine.TotalGameTime.Ticks);

            while (physicsBody.Accumulator >= target)
            {
                Integrate(entity, integrator, target);
                physicsBody.Accumulator -= target;
            }
        }

        private void Integrate(int entity, Integrator integrator, float deltaTime)
        {
            CPosition position = scene.GetData<CPosition>(entity);
            CPhysicsBody physicsBody = scene.GetData<CPhysicsBody>(entity);

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

            position.X += physicsBody.Velocity.X * deltaTime;
            position.Y += physicsBody.Velocity.Y * deltaTime;
        }

        private static void VelocityVerletIntegration(float deltaTime, CPosition position, CPhysicsBody physicsBody)
        {
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