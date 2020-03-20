using Example.Components;
using Microsoft.Xna.Framework;
using Morro.ECS;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class SHunting : UpdateSystem
    {
        private IComponent[] positions;
        private IComponent[] predators;
        private IComponent[] physicsBodies;
        private IComponent[] transforms;
        private IComponent[] boids;

        public SHunting(Scene scene) : base(scene, 4, 60)
        {
            Require(typeof(CPosition), typeof(CPredator), typeof(CPhysicsBody), typeof(CTransform), typeof(CBoid));
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CPredator predator = (CPredator)predators[entity];
            CPhysicsBody physicsBody = (CPhysicsBody)physicsBodies[entity];
            CTransform transform = (CTransform)transforms[entity];
            CBoid boid = (CBoid)boids[entity];

            if (!predator.Seeking)
            {
                predator.Seeking = true;
                predator.Target = new Vector2(Morro.Maths.MoreRandom.Range(0, (int)scene.SceneBounds.Width), Morro.Maths.MoreRandom.Range(0, (int)scene.SceneBounds.Height));
            }
            else
            {
                Vector2 positionVector = new Vector2(position.X, position.Y);

                if (Vector2.Distance(positionVector, predator.Target) < 8)
                {
                    predator.Seeking = false;
                    return;
                }

                Vector2 desired = predator.Target - positionVector;
                desired.SetMagnitude(boid.MoveSpeed);
                Vector2 seek = desired - physicsBody.Velocity;
                seek.Limit(boid.MaxForce);
                physicsBody.Velocity += seek;

                transform.Rotation = -(float)Math.Atan2(physicsBody.Velocity.Y, physicsBody.Velocity.X);
            }
        }

        public override void Update()
        {
            positions = scene.GetData<CPosition>();
            predators = scene.GetData<CPredator>();
            physicsBodies = scene.GetData<CPhysicsBody>();
            transforms = scene.GetData<CTransform>();
            boids = scene.GetData<CBoid>();

            base.Update();
        }
    }
}
