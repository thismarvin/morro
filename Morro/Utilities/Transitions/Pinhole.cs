using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Graphics;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Utilities
{
    class Pinhole : Transition
    {
        const int PADDING = 64;

        private readonly Circle pinhole;
        private int radius;

        public Pinhole(TransitionType type) : this(type, 0, 4)
        {

        }

        public Pinhole(TransitionType type, float speed, float acceleration) : base(type, speed, acceleration)
        {
            pinhole = new Circle(0, 0, 1, Color.Black, VertexInformation.Dynamic);
        }

        protected override void SetupTransition()
        {
            int lineWidth = Type == TransitionType.Enter ? radius : 1;
            pinhole.SetLineWidth(lineWidth);
        }

        protected override void AccommodateToCamera()
        {
            radius = (int)Camera.Bounds.Width > (int)Camera.Bounds.Height ? (int)Camera.Bounds.Width / 2 : (int)Camera.Bounds.Height / 2;
            radius += PADDING;
            pinhole.SetRadius(radius);
            pinhole.SetPosition(Camera.Bounds.X + Camera.Bounds.Width / 2, Camera.Bounds.Y + Camera.Bounds.Height / 2);
        }

        protected override void UpdateLogic()
        {
            switch (Type)
            {
                case TransitionType.Enter:
                    pinhole.SetLineWidth(pinhole.LineWidth - Force);
                    if (pinhole.LineWidth <= 1)
                    {
                        pinhole.SetLineWidth(1);
                        FlagCompletion();
                    }
                    break;

                case TransitionType.Exit:
                    pinhole.SetLineWidth(pinhole.LineWidth + Force);
                    if (pinhole.LineWidth >= radius)
                    {
                        pinhole.SetLineWidth(radius);
                        FlagCompletion();
                    }
                    break;
            }
        }

        protected override void DrawTransition(SpriteBatch spriteBatch)
        {
            pinhole.Draw(spriteBatch, CameraType.Static);
        }
    }
}
