using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS.Scenes
{
    class SAT : Scene
    {
        Line line1;
        Line line2;
        //Line line3;

        RegularPolygon test;

        AABB test1;
        AABB test2;

        Vector2 dir;

        public SAT() : base(SceneType.SAT)
        {

        }

        protected override void Initialize()
        {

        }

        public override void LoadScene()
        {
            //line1 = new Line(0, SceneBounds.Height / 2, SceneBounds.Width / 2 - 64, SceneBounds.Height / 2, 1, Color.Black, VertexInformation.Dynamic);
            //line2 = new Line(SceneBounds.Width / 2, 0, SceneBounds.Width / 2, SceneBounds.Height, 1, Color.Black, VertexInformation.Dynamic);
            //test = new RegularPolygon(0, 0, 4, 4, 6, Color.Red, VertexInformation.Static);
            //line3 = new Line(0, 1, 0, 1, 1, Color.Green, VertexInformation.Dynamic);

            dir = new Vector2 (0, 0);
            test1 = new AABB(64, 64, 16, 16, Color.White, VertexInformation.Static);
            test2 = new AABB(128, 64, 64, 64, Color.Red, VertexInformation.Static);

        }

        public override void UnloadScene()
        {

        }

        public override void Update(GameTime gameTime)
        {
            //line1.SetEndPoint(Input.Mouse.DynamicLocation.X, Input.Mouse.DynamicLocation.Y);

            //Vector2 a = line1.EndPoint - line1.StartingPoint;
            //Vector2 b = line2.EndPoint - line2.StartingPoint;

            //float theta = (float)Math.Acos(Vector2.Dot(a, b) / (a.Length() * b.Length()));
            //float theta1 = (float)Math.Atan2(line1.EndPoint.Y - line2.EndPoint.Y, line1.EndPoint.X - line2.StartingPoint.X);

            //b.Normalize();
            //float scale = Vector2.Dot(a, b);
            //b.SetMagnitude(scale);

            //test.SetLocation(line2.X + b.X, line2.Y + b.Y);
            //line3.SetEndPoint(line1.EndPoint.X, line1.EndPoint.Y);
            //line3.SetStartingPoint(test.X, test.Y);

            if (Input.Keyboard.Pressing(Keys.W))
            {
                dir = new Vector2(dir.X, -1);
            }
            else if (Input.Keyboard.Pressing(Keys.S))
            {
                dir = new Vector2(dir.X, 1);
            }
            else
                dir = new Vector2(dir.X, 0);
            if (Input.Keyboard.Pressing(Keys.A))
            {
                dir = new Vector2(-1, dir.Y);
            }
            else if (Input.Keyboard.Pressing(Keys.D))
            {
                dir = new Vector2(1, dir.Y);
            }
            else
                dir = new Vector2(0, dir.Y);

            if (dir.X != 0 || dir.Y != 0)
                dir.SetMagnitude(50);


            test1.SetLocation(
                test1.X + dir.X * Engine.DeltaTime,
                test1.Y + dir.Y * Engine.DeltaTime
            );


            //Console.WriteLine(Intersects(test1, test2));
            ResolveCollision(test1, test2, dir);

        }

        private bool Intersects(Polygon a, Polygon b)
        {
            a.SetupForCollisionTesting();
            b.SetupForCollisionTesting();

            return SATCheck(a, b) && SATCheck(b, a);
        }

        private bool SATCheck(Polygon a, Polygon b)
        {
            Vector2 normal;
            float projection;
            float minProjectionA;
            float maxProjectionA;
            float minProjectionB;
            float maxProjectionB;

            for (int i = 0; i < a.LineSegments.Length; i++)
            {
                normal = new Vector2(
                    -(a.LineSegments[i].Y2 - a.LineSegments[i].Y1),
                      a.LineSegments[i].X2 - a.LineSegments[i].X1
                    );

                minProjectionA = Vector2.Dot(a.TransformedVertices[0], normal);
                maxProjectionA = minProjectionA;
                for (int j = 0; j < a.TransformedVertices.Length; j++)
                {
                    projection = Vector2.Dot(a.TransformedVertices[j], normal);
                    minProjectionA = Math.Min(minProjectionA, projection);
                    maxProjectionA = Math.Max(maxProjectionA, projection);
                }

                minProjectionB = Vector2.Dot(b.TransformedVertices[0], normal);
                maxProjectionB = minProjectionB;
                for (int j = 0; j < b.TransformedVertices.Length; j++)
                {
                    projection = Vector2.Dot(b.TransformedVertices[j], normal);
                    minProjectionB = Math.Min(minProjectionB, projection);
                    maxProjectionB = Math.Max(maxProjectionB, projection);
                }

                if (!(maxProjectionB >= minProjectionA && maxProjectionA >= minProjectionB))
                    return false;
            }
            return true;
        }


        private void ResolveCollision(Polygon a, Polygon b, Vector2 velocity)
        {
            a.SetupForCollisionTesting();
            b.SetupForCollisionTesting();

            MTV pass1 = SATResolve(a, b);
            MTV pass2 = SATResolve(b, a);

            if (pass1.Overlap == -1 || pass2.Overlap == -1)
                return;

            Console.WriteLine(pass1.EdgeIndex);
            
            if (pass1.Overlap < pass2.Overlap)
            {
                Vector2 axis = new Vector2(-(pass1.Edge.Y2 - pass1.Edge.Y1), pass1.Edge.X2 - pass1.Edge.X1);
                float edgeLength = axis.Length();
                float angle = (float)Math.Acos(Vector2.Dot(axis, Vector2.UnitX) / (edgeLength));

                float xFactor = (float)Math.Round(edgeLength * Math.Cos(angle));
                float yFactor = (float)Math.Round(edgeLength * Math.Sin(angle));

                float xOffset = xFactor == 0 ? 0 : pass1.Overlap / xFactor;
                float yOffset = yFactor == 0 ? 0 : pass1.Overlap / yFactor;

                if (velocity.X < 0)
                    xOffset *= -1;
                if (velocity.Y > 0)
                    yOffset *= -1;
                
                a.SetLocation(a.X + xOffset, a.Y + yOffset);
            }
            else
            {
                Vector2 axis = new Vector2(-(pass2.Edge.Y2 - pass2.Edge.Y1), pass2.Edge.X2 - pass2.Edge.X1);
                float edgeLength = axis.Length();
                float angle = (float)Math.Acos(Vector2.Dot(axis, Vector2.UnitX) / (edgeLength * Vector2.UnitX.Length()));

                float xFactor = (float)Math.Round(edgeLength * Math.Cos(angle));
                float yFactor = (float)Math.Round(edgeLength * Math.Sin(angle));

                float xOffset = xFactor == 0 ? 0 : pass2.Overlap / xFactor;
                float yOffset = yFactor == 0 ? 0 : pass2.Overlap / yFactor;

                if (velocity.X < 0)
                    xOffset *= -1;
                if (velocity.Y > 0)
                    yOffset *= -1;

                a.SetLocation(a.X + xOffset, a.Y + yOffset);
            }
        }

        private MTV SATResolve(Polygon a, Polygon b)
        {
            LineSegment edge = new LineSegment();
            float minOverlap = float.MaxValue;
            int axisIndex = -1;
            float overlap;
            
            Vector2 normal;
            float projection;
            float minProjectionA;
            float maxProjectionA;
            float minProjectionB;
            float maxProjectionB;
            for (int i = 0; i < a.LineSegments.Length; i++)
            {
                normal = new Vector2(
                    -(a.LineSegments[i].Y2 - a.LineSegments[i].Y1),
                      a.LineSegments[i].X2 - a.LineSegments[i].X1
                    );

                minProjectionA = Vector2.Dot(a.TransformedVertices[0], normal);
                maxProjectionA = minProjectionA;
                for (int j = 0; j < a.TransformedVertices.Length; j++)
                {
                    projection = Vector2.Dot(a.TransformedVertices[j], normal);
                    minProjectionA = Math.Min(minProjectionA, projection);
                    maxProjectionA = Math.Max(maxProjectionA, projection);
                }

                minProjectionB = Vector2.Dot(b.TransformedVertices[0], normal);
                maxProjectionB = minProjectionB;
                for (int j = 0; j < b.TransformedVertices.Length; j++)
                {
                    projection = Vector2.Dot(b.TransformedVertices[j], normal);
                    minProjectionB = Math.Min(minProjectionB, projection);
                    maxProjectionB = Math.Max(maxProjectionB, projection);
                }

                overlap = Math.Min(maxProjectionA, maxProjectionB) - Math.Max(minProjectionA, minProjectionB);
                //overlap = Math.Abs(overlap);

                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    edge = a.LineSegments[i];
                    axisIndex = i;
                }
               
                if (!(maxProjectionB >= minProjectionA && maxProjectionA >= minProjectionB))
                    return new MTV(new LineSegment(), -1, -1);
            }

            return new MTV(edge, minOverlap, axisIndex);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //line1.Draw(spriteBatch, CameraType.Dynamic);
            //line2.Draw(spriteBatch, CameraType.Dynamic);
            //test.Draw(spriteBatch, CameraType.Dynamic);


            //line3.Draw(spriteBatch, CameraType.Dynamic);
            //test.Draw(spriteBatch, CameraType.Dynamic);

            test1.Draw(spriteBatch, CameraType.Dynamic);
            test2.Draw(spriteBatch, CameraType.Dynamic);

        }
    }
}



