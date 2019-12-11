using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Utilities
{
    class Pinhole : Transition
    {
        private readonly Circle pinhole;
        private readonly int size;

        private bool setup; 

        public Pinhole(TransitionType type) : this(type, 50, 500)
        {

        }

        public Pinhole(TransitionType type, float speed, float jerk) : base(type)
        {
            this.speed = speed;
            this.jerk = jerk;
            size = Width > Height ? Width / 2 : Height / 2;

            switch (Type)
            {
                case TransitionType.Enter:
                    pinhole = new Circle(X, Y, size, size, Color.Black, VertexInformation.Dynamic);
                    break;
                case TransitionType.Exit:
                    pinhole = new Circle(X, Y, size, 1, Color.Black, VertexInformation.Dynamic);
                    break;
            }            
        }

        public override void Reset()
        {
            base.Reset();
            setup = false;
            switch (Type)
            {
                case TransitionType.Enter:
                    pinhole.SetLineWidth(size);
                    break;

                case TransitionType.Exit:
                    pinhole.SetLineWidth(1);
                    break;
            }
        }

        public override void SetLocation(float x, float y)
        {
            base.SetLocation(x, y);
            pinhole.SetLocation(X, Y);
        }

        public override void Update()
        {
            if (Done)
                return;
            
            CalculateForce();

            switch (Type)
            {
                case TransitionType.Enter:
                    pinhole.SetLineWidth(pinhole.LineWidth - velocity * Engine.DeltaTime);
                    if (pinhole.LineWidth <= 1)
                    {
                        pinhole.SetLineWidth(1);
                        lastDraw = true;
                    }
                    break;

                case TransitionType.Exit:
                    pinhole.SetLineWidth(pinhole.LineWidth + velocity * Engine.DeltaTime);
                    if (pinhole.LineWidth >= size)
                    {
                        pinhole.SetLineWidth(size);
                        lastDraw = true;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!setup)
            {
                Camera camera = CameraManager.GetCamera(CameraType.Static);
                Core.Rectangle bounds = camera.Bounds;
                int scaledRadius = bounds.Width > bounds.Height ? bounds.Width / 2 : bounds.Height / 2;

                pinhole.SetRadius(scaledRadius + BUFFER);
                pinhole.SetLineWidth(pinhole.Radius);
                pinhole.SetLocation(camera.Bounds.X + camera.Bounds.Width / 2, camera.Bounds.Y + camera.Bounds.Height / 2);

                jerk = scaledRadius;
                setup = true;                
            }

            if (Done)
                return;
            
            pinhole.Draw(spriteBatch, CameraType.Static);

            if (lastDraw)
                Done = true;
        }
    }
}
