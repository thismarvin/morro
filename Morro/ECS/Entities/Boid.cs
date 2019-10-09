using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Graphics;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class Boid : Kinetic
    {
        private readonly RegularPolygon body;
        private readonly float viewRadius;
        private readonly float maxForce;
        private float angle;

        public Boid(float x, float y) : base(x, y, 4, 2, 1)
        {
            body = new RegularPolygon(X, Y, Width, Height, 3, Color, VertexInformation.Static);
            MoveSpeed = RandomHelper.Range(40, 50);
            viewRadius = 16;
            maxForce = 0.5f;

            Velocity = new Vector2(
            (-MoveSpeed + RandomHelper.Range(0, 5) * MoveSpeed * 2 / 5) * 0.01f,
            (-MoveSpeed + RandomHelper.Range(0, 5) * MoveSpeed * 2 / 5) * 0.01f
            );

            SetColor(Color.Black);

            body.SetRotationOffset(body.Width / 2, body.Height / 2);
        }

        public override void SetColor(Color color)
        {
            base.SetColor(color);
            body.SetColor(Color);
        }

        public override void SetLocation(float x, float y)
        {
            base.SetLocation(x, y);
            body.SetLocation(X, Y);
        }

        protected override void Collision()
        {
            if (Bounds.Right < 0)
            {
                SetLocation(SceneManager.CurrentScene.SceneBounds.Width, Y);
            }
            else if (Bounds.Left > SceneManager.CurrentScene.SceneBounds.Width)
            {
                SetLocation(0 - Width, Y);
            }

            if (Bounds.Bottom < 0)
            {
                SetLocation(X, SceneManager.CurrentScene.SceneBounds.Height);
            }
            else if (Bounds.Top > SceneManager.CurrentScene.SceneBounds.Height)
            {
                SetLocation(X, 0 - Height);
            }
        }

        private Vector2 Seperation(List<MonoObject> queryResult)
        {
            Vector2 result = Vector2.Zero;
            Vector2 cumulative = Vector2.Zero;
            Vector2 force;
            int total = 0;
            float distance;

            for (int i = 0; i < queryResult.Count; i++)
            {
                if (queryResult[i] == this)
                    continue;

                if (!(queryResult[i] is Boid))
                    continue;

                distance = Vector2.Distance(Center, queryResult[i].Center);

                if (distance > 0 && distance < queryResult[i].Width * 0.75f)
                {
                    force = Center - queryResult[i].Center;
                    force /= distance * distance;
                    cumulative += force;
                    total++;
                }
            }

            if (total > 0)
            {
                cumulative /= total;
                cumulative.SetMagnitude(MoveSpeed);
                result = cumulative - Velocity;
                result.Limit(maxForce);
            }

            return result;
        }

        private Vector2 Alignment(List<MonoObject> queryResult)
        {
            Vector2 result = Vector2.Zero;
            Vector2 cumulative = Vector2.Zero;
            int total = 0;
            float distance;

            for (int i = 0; i < queryResult.Count; i++)
            {
                if (queryResult[i] == this)
                    continue;

                if (!(queryResult[i] is Boid))
                    continue;

                distance = Vector2.Distance(Center, queryResult[i].Center);

                if (distance > 0 && distance < viewRadius)
                {
                    cumulative += ((Boid)queryResult[i]).Velocity;
                    total++;
                }
            }

            if (total > 0)
            {
                cumulative /= total;
                cumulative.SetMagnitude(MoveSpeed);
                result = cumulative - Velocity;
                result.Limit(maxForce);
            }

            return result;
        }

        private Vector2 Cohesion(List<MonoObject> queryResult)
        {
            Vector2 result = Vector2.Zero;
            Vector2 cumulative = Vector2.Zero;
            int total = 0;
            float distance;

            for (int i = 0; i < queryResult.Count; i++)
            {
                if (queryResult[i] == this)
                    continue;

                if (!(queryResult[i] is Boid))
                    continue;

                distance = Vector2.Distance(Center, queryResult[i].Center);

                if (distance > 0 && distance < viewRadius)
                {
                    cumulative += queryResult[i].Center;
                    total++;
                }
            }

            if (total > 0)
            {
                cumulative /= total;
                cumulative -= Center;
                cumulative.SetMagnitude(MoveSpeed);
                result = cumulative - Velocity;
                result.Limit(maxForce);
            }

            return result;
        }

        private void UpdateFlocking()
        {
            Core.Rectangle queryBounds =
                new Core.Rectangle(
                    X - viewRadius,
                    Y - viewRadius,
                    (int)viewRadius * 2 + Width,
                    (int)viewRadius * 2 + Height
                );

            List<MonoObject> queryResult = SceneManager.CurrentScene.EntityBin.Query(queryBounds);

            Vector2 separation = Seperation(queryResult);
            Vector2 alignment = Alignment(queryResult);
            Vector2 cohesion = Cohesion(queryResult);

            separation *= 3;
            alignment *= 1.5f;
            cohesion *= 0.75f;

            Velocity += separation + alignment + cohesion;

            angle = (float)Math.Atan2(Velocity.Y, Velocity.X);
            body.SetRotation(angle + MathHelper.Pi);
        }

        public override void Update(GameTime gameTime)
        {
            ApplyForce(Integrator.VelocityVerlet);
            Collision();
            UpdateFlocking();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            body.Draw(spriteBatch, CameraType.Dynamic);
        }
    }
}
