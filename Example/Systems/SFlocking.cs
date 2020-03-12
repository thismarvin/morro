using Example.Components;
using Microsoft.Xna.Framework;
using Morro.ECS;
using Morro.Maths;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Systems
{
    class SFlocking : UpdateSystem
    {
        private IComponent[] boids;
        private IComponent[] positions;
        private IComponent[] dimensions;
        private IComponent[] transforms;
        private IComponent[] physicsBodies;

        private readonly float seperationIntensity;
        private readonly float alignmentIntensity;
        private readonly float cohesionIntensity;

        private SBinPartitioner binPartitioner;

        public SFlocking(Scene scene) : base(scene, 4)
        {
            Require(typeof(CBoid), typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CPhysicsBody));
            Depend(typeof(SBinPartitioner));

            seperationIntensity = 3;
            alignmentIntensity = 1.5f;
            cohesionIntensity = 0.75f;
        }

        public override void UpdateEntity(int entity)
        {
            CBoid boid = (CBoid)boids[entity];
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];
            CTransform transform = (CTransform)transforms[entity];
            CPhysicsBody physicsBody = (CPhysicsBody)physicsBodies[entity];

            Vector2 myCenter = new Vector2(position.X + dimension.Width / 2, position.Y + dimension.Height / 2);

            Vector2 cumulativeSeperation = Vector2.Zero;
            int totalSeperation = 0;

            Vector2 cumulativeAlignment = Vector2.Zero;
            int totalAlignment = 0;

            Vector2 cumulativeCohesion = Vector2.Zero;
            int totalCohesion = 0;


            float distance;
            CPosition theirPosition;
            CDimension theirDimension;
            CPhysicsBody theirPhysicsBody;
            Vector2 force;
            Vector2 theirCenter;

            HashSet<int> queryResult = binPartitioner.Query(new Morro.Core.Rectangle(position.X - boid.ViewRadius, position.Y - boid.ViewRadius, dimension.Width + boid.ViewRadius * 2, dimension.Height + boid.ViewRadius * 2));

            foreach (int queriedEntity in queryResult)
            {
                if (queriedEntity == entity)
                    continue;

                theirPosition = (CPosition)positions[queriedEntity];
                theirDimension = (CDimension)dimensions[queriedEntity];
                theirPhysicsBody = (CPhysicsBody)physicsBodies[queriedEntity];

                theirCenter = new Vector2(theirPosition.X + theirDimension.Width / 2, theirPosition.Y + theirDimension.Height / 2);

                distance = Vector2.Distance(myCenter, theirCenter);

                if (distance > 0)
                {
                    if (distance < boid.ViewRadius)
                    {
                        cumulativeCohesion += theirCenter;
                        totalCohesion++;

                        cumulativeAlignment += theirPhysicsBody.Velocity;
                        totalAlignment++;
                    }

                    if (distance < theirDimension.Width * 2)
                    {
                        force = myCenter - theirCenter;
                        force /= distance * distance;
                        cumulativeSeperation += force;
                        totalSeperation++;
                    }
                }
            }

            distance = Vector2.Distance(Morro.Input.Mouse.SceneLocation, myCenter);

            if (distance > 0 && distance < 32)
            {
                force = myCenter - Morro.Input.Mouse.SceneLocation;
                force /= distance * distance;
                cumulativeSeperation += force;
                totalSeperation++;
            }

            Vector2 seperation = CalculateSeperation();
            Vector2 alignment = CalculateAlignment();
            Vector2 cohesion = CalculateCohesion();

            Vector2 totalForce = seperation + alignment + cohesion;

            physicsBody.Velocity += totalForce;
            //transform.Rotation = -(float)Math.Atan2(physicsBody.Velocity.Y, physicsBody.Velocity.X);
            transform.Rotation = MathHelper.Lerp(transform.Rotation, -(float)Math.Atan2(physicsBody.Velocity.Y, physicsBody.Velocity.X), 0.9f);

            Vector2 CalculateSeperation()
            {
                if (totalSeperation <= 0)
                    return Vector2.Zero;

                cumulativeSeperation /= totalSeperation;
                cumulativeSeperation.SetMagnitude(boid.MoveSpeed);
                Vector2 result = cumulativeSeperation - physicsBody.Velocity;
                result.Limit(boid.MaxForce);

                return result * seperationIntensity;
            }

            Vector2 CalculateAlignment()
            {
                if (totalAlignment <= 0)
                    return Vector2.Zero;

                cumulativeAlignment /= totalAlignment;
                cumulativeAlignment.SetMagnitude(boid.MoveSpeed);
                Vector2 result = cumulativeAlignment - physicsBody.Velocity;
                result.Limit(boid.MaxForce);

                return result * alignmentIntensity;
            }

            Vector2 CalculateCohesion()
            {
                if (totalCohesion <= 0)
                    return Vector2.Zero;

                cumulativeCohesion /= totalCohesion;
                cumulativeCohesion -= myCenter;
                cumulativeCohesion.SetMagnitude(boid.MoveSpeed);
                Vector2 result = cumulativeCohesion - physicsBody.Velocity;
                result.Limit(boid.MaxForce);

                return result * cohesionIntensity;
            }
        }

        public override void Update()
        {
            boids = scene.GetData<CBoid>();
            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();
            transforms = scene.GetData<CTransform>();
            physicsBodies = scene.GetData<CPhysicsBody>();

            if (binPartitioner == null)
            {
                binPartitioner = scene.GetSystem<SBinPartitioner>();
            }

            base.Update();
        }
    }
}
