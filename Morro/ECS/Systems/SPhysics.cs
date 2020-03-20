﻿using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    /// <summary>
    /// The types of integrators available to the built in physics system.
    /// </summary>
    public enum Integrator
    {
        SemiImplictEuler,
        VelocityVerlet
    }

    class SPhysics : UpdateSystem
    {
        private readonly float target;
        private readonly Integrator integrator;

        private IComponent[] positions;
        private IComponent[] kinetics;

        public SPhysics(Scene scene, Integrator integrator, uint tasks, int targetFPS) : base(scene, tasks)
        {
            Require(typeof(CPosition), typeof(CKinetic));

            target = 1f / targetFPS;
            this.integrator = integrator;
        }

        public override void UpdateEntity(int entity)
        {
            Simultate(entity);
        }

        public override void Update()
        {
            positions = scene.GetData<CPosition>();
            kinetics = scene.GetData<CKinetic>();

            base.Update();
        }

        private void Simultate(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CKinetic kinetic = (CKinetic)kinetics[entity];

            kinetic.Accumulator += (float)(Engine.TotalGameTime - kinetic.LastUpdate).TotalSeconds;
            kinetic.LastUpdate = new TimeSpan(Engine.TotalGameTime.Ticks);

            while (kinetic.Accumulator >= target)
            {
                Integrate(position, kinetic, integrator, target);
                kinetic.Accumulator -= target;
            }
        }

        private void Integrate(CPosition position, CKinetic kinetic, Integrator integrator, float deltaTime)
        {
            switch (integrator)
            {
                case Integrator.SemiImplictEuler:
                    SemiImplictEulerIntegration();
                    break;

                case Integrator.VelocityVerlet:
                    VelocityVerletIntegration();
                    break;
            }

            void SemiImplictEulerIntegration()
            {
                kinetic.Velocity += kinetic.Acceleration * deltaTime;

                position.X += kinetic.Velocity.X * deltaTime;
                position.Y += kinetic.Velocity.Y * deltaTime;
            }

            void VelocityVerletIntegration()
            {
                position.X += kinetic.Velocity.X * deltaTime + 0.5f * kinetic.Acceleration.X * deltaTime * deltaTime;
                position.Y += kinetic.Velocity.Y * deltaTime + 0.5f * kinetic.Acceleration.Y * deltaTime * deltaTime;

                kinetic.Velocity += kinetic.Acceleration * deltaTime;
            }
        }
    }
}
