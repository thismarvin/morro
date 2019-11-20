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

        public Pinhole(TransitionType type) : this(type, 100, 500)
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

        public void SetSpeed(float speed)
        {
            velocity = velocity < 0 ? -speed : speed;
        }

        public override void Update()
        {
            if (!InProgress)
                return;

            CalculateForce();

            switch (Type)
            {
                case TransitionType.Enter:
                    pinhole.SetLineWidth(pinhole.LineWidth - velocity);
                    if (pinhole.LineWidth <= 1)
                    {
                        pinhole.SetLineWidth(1);
                        lastDraw = true;
                    }
                    break;

                case TransitionType.Exit:
                    pinhole.SetLineWidth(pinhole.LineWidth + velocity);
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
            if (!InProgress)
                return;

            pinhole.Draw(spriteBatch, CameraType.Static);

            if (lastDraw)
                Done = true;
        }
    }
}
