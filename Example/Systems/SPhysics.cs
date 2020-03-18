using System;
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
    class SPhysics : UpdateSystem
    {
        private readonly float target;
        private readonly Integrator integrator;

        private IComponent[] positions;
        private IComponent[] physicsBodies;

        private enum Integrator
        {
            SemiImplictEuler,
            VelocityVerlet
        }

        public SPhysics(Scene scene) : base(scene, 4)
        {
            Require(typeof(CPosition), typeof(CPhysicsBody));
            Depend(typeof(SFlocking));

            target = 1f / 60;
            integrator = Integrator.SemiImplictEuler;
        }

        public override void UpdateEntity(int entity)
        {
            Simultate(entity);
        }

        public override void Update()
        {
            positions = scene.GetData<CPosition>();
            physicsBodies = scene.GetData<CPhysicsBody>();

            base.Update();
        }

        private void Simultate(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CPhysicsBody physicsBody = (CPhysicsBody)physicsBodies[entity];

            physicsBody.Accumulator += (float)(Engine.TotalGameTime - physicsBody.LastUpdate).TotalSeconds;
            physicsBody.LastUpdate = new TimeSpan(Engine.TotalGameTime.Ticks);

            while (physicsBody.Accumulator >= target)
            {
                Integrate(position, physicsBody, integrator, target);
                physicsBody.Accumulator -= target;
            }
        }

        private void Integrate(CPosition position, CPhysicsBody physicsBody, Integrator integrator, float deltaTime)
        {
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
            physicsBody.Velocity += physicsBody.Acceleration * deltaTime;

            position.X += physicsBody.Velocity.X * deltaTime;
            position.Y += physicsBody.Velocity.Y * deltaTime;
        }

        private static void VelocityVerletIntegration(float deltaTime, CPosition position, CPhysicsBody physicsBody)
        {
            position.X += physicsBody.Velocity.X * deltaTime + 0.5f * physicsBody.Acceleration.X * deltaTime * deltaTime;
            position.Y += physicsBody.Velocity.Y * deltaTime + 0.5f * physicsBody.Acceleration.Y * deltaTime * deltaTime;

            physicsBody.Velocity += physicsBody.Acceleration * deltaTime;
        }
    }
}
