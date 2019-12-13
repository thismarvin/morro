using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Utilities
{
    class Fade : Transition
    {
        const int PADDING = 8;

        private float alpha;
        private Color defaultColor;
        private Color fadeColor;
        private readonly Quad fade;

        public Fade(TransitionType type) : this(type, 0.01f, 0.01f, Color.Black)
        {

        }

        public Fade(TransitionType type, float velocity, float acceleration, Color color) : base(type, velocity, acceleration)
        {
            defaultColor = color;
            fade = new Quad(-PADDING, -PADDING, 1, 1, Color.Black, VertexInformation.Dynamic);
        }

        private void AccommodateToCamera()
        {
            Camera camera = CameraManager.GetCamera(CameraType.Static);
            fade.SetBounds
            (
                -WindowManager.PillarBox - PADDING,
                -WindowManager.LetterBox - PADDING,
                camera.Bounds.Width + PADDING * 2,
                camera.Bounds.Height + PADDING * 2
            );
        }

        private void CalculateColor()
        {
            alpha = Type == TransitionType.Enter ? 1 : 0;
            fadeColor = new Color(defaultColor, alpha);
            fade.SetColor(fadeColor);
        }

        private void Setup()
        {
            setup = true;
            AccommodateToCamera();
            CalculateColor();
        }

        public override void Update()
        {
            if (Done || !setup)
                return;

            CalculateForce();

            if (WindowManager.WideScreenSupported)
            {
                AccommodateToCamera();
            }

            switch (Type)
            {
                case TransitionType.Exit:
                    alpha += Force;
                    if (alpha > 1)
                    {
                        alpha = 1;
                        lastDraw = true;
                    }
                    break;

                case TransitionType.Enter:
                    alpha -= Force;
                    if (alpha < 0)
                    {
                        alpha = 0;
                        lastDraw = true;
                    }
                    break;
            }

            fadeColor = new Color(defaultColor, alpha);
            fade.SetColor(fadeColor);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!setup)
                Setup();

            if (Done)
                return;

            fade.Draw(spriteBatch, CameraType.Static);

            if (lastDraw)
                Done = true;
        }
    }
}
