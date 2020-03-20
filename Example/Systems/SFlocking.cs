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
        private IComponent[] kinetics;

        private readonly float seperationIntensity;
        private readonly float alignmentIntensity;
        private readonly float cohesionIntensity;

        private SBinPartitioner binPartitioner;

        public SFlocking(Scene scene) : base(scene, 4, 60)
        {
            Require(typeof(CBoid), typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CKinetic));
            Avoid(typeof(CPredator));
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
            CKinetic kinetic = (CKinetic)kinetics[entity];

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
            CKinetic theirPhysicsBody;
            Vector2 force;
            Vector2 theirCenter;

            List<int> queryResult = binPartitioner.Query(new Morro.Core.RectangleF(position.X - boid.ViewRadius, position.Y - boid.ViewRadius, dimension.Width + boid.ViewRadius * 2, dimension.Height + boid.ViewRadius * 2));

            for (int i = 0; i < queryResult.Count; i++)
            {
                if (queryResult[i] == entity)
                    continue;

                theirPosition = (CPosition)positions[queryResult[i]];
                theirDimension = (CDimension)dimensions[queryResult[i]];
                theirPhysicsBody = (CKinetic)kinetics[queryResult[i]];

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

                    if (distance < 64 && theirDimension.Width > 2)
                    {
                        force = myCenter - theirCenter;
                        force /= distance * distance;
                        cumulativeSeperation += force * 4;
                        totalSeperation++;
                    }
                }
            }

            distance = Vector2.Distance(Morro.Input.MMouse.SceneLocation, myCenter);

            if (distance > 0 && distance < 32)
            {
                force = myCenter - Morro.Input.MMouse.SceneLocation;
                force /= distance * distance;
                cumulativeSeperation += force;
                totalSeperation++;
            }

            Vector2 seperation = CalculateSeperation();
            Vector2 alignment = CalculateAlignment();
            Vector2 cohesion = CalculateCohesion();

            Vector2 totalForce = seperation + alignment + cohesion;

            kinetic.Velocity += totalForce;
            transform.Rotation = -(float)Math.Atan2(kinetic.Velocity.Y, kinetic.Velocity.X);

            Vector2 CalculateSeperation()
            {
                if (totalSeperation <= 0)
                    return Vector2.Zero;

                cumulativeSeperation /= totalSeperation;
                cumulativeSeperation.SetMagnitude(boid.MoveSpeed);
                Vector2 result = cumulativeSeperation - kinetic.Velocity;
                result.Limit(boid.MaxForce);

                return result * seperationIntensity;
            }

            Vector2 CalculateAlignment()
            {
                if (totalAlignment <= 0)
                    return Vector2.Zero;

                cumulativeAlignment /= totalAlignment;
                cumulativeAlignment.SetMagnitude(boid.MoveSpeed);
                Vector2 result = cumulativeAlignment - kinetic.Velocity;
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
                Vector2 result = cumulativeCohesion - kinetic.Velocity;
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
            kinetics = scene.GetData<CKinetic>();

            if (binPartitioner == null)
            {
                binPartitioner = scene.GetSystem<SBinPartitioner>();
            }

            base.Update();
        }
    }
}
