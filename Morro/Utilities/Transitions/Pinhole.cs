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
        const int PADDING = 64;

        private Circle pinhole;
        private int radius;

        public Pinhole(TransitionType type) : this(type, 1, 5)
        {

        }

        public Pinhole(TransitionType type, float speed, float acceleration) : base(type, speed, acceleration)
        {
            pinhole = new Circle(0, 0, 1, Color.Black, VertexInformation.Dynamic);
        }

        private void AccommodateToCamera()
        {
            Camera camera = CameraManager.GetCamera(CameraType.Static);
            radius = camera.Bounds.Width > camera.Bounds.Height ? camera.Bounds.Width / 2 : camera.Bounds.Height / 2;
            radius += PADDING;
            pinhole.SetRadius(radius);
            pinhole.SetLocation(camera.Bounds.X + camera.Bounds.Width / 2, camera.Bounds.Y + camera.Bounds.Height / 2);
        }

        private void CalculateLineWidth()
        {
            int lineWidth = Type == TransitionType.Enter ? radius : 1;
            pinhole.SetLineWidth(lineWidth);
        }

        private void Setup()
        {
            setup = true;
            AccommodateToCamera();
            CalculateLineWidth();
        }

        public override void Update()
        {
            if (Done || !setup)
                return;

            CalculateForce();
            AccommodateToCamera();

            switch (Type)
            {
                case TransitionType.Enter:
                    pinhole.SetLineWidth(pinhole.LineWidth - Force);
                    if (pinhole.LineWidth <= 1)
                    {
                        pinhole.SetLineWidth(1);
                        lastDraw = true;
                    }
                    break;

                case TransitionType.Exit:
                    pinhole.SetLineWidth(pinhole.LineWidth + Force);
                    if (pinhole.LineWidth >= radius)
                    {
                        pinhole.SetLineWidth(radius);
                        lastDraw = true;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!setup)
                Setup();

            if (Done)
                return;

            pinhole.Draw(spriteBatch, CameraType.Static);

            if (lastDraw)
                Done = true;
        }
    }
}
