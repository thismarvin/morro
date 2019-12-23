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

        protected override void AccommodateToCamera()
        {
            fade.SetDimensions(Camera.Bounds.Width + PADDING * 2, Camera.Bounds.Height + PADDING * 2);

            if (WindowManager.WideScreenSupported)
            {
                fade.SetPosition(-WindowManager.PillarBox - PADDING, -WindowManager.LetterBox - PADDING);
            }
            else
            {
                fade.SetPosition(-PADDING, -PADDING);
            }
        }

        protected override void SetupTransition()
        {
            alpha = Type == TransitionType.Enter ? 1 : 0;
            fadeColor = defaultColor * alpha;
            fade.SetColor(fadeColor);
        }

        protected override void UpdateLogic()
        {
            switch (Type)
            {
                case TransitionType.Exit:
                    alpha += Force;
                    if (alpha > 1)
                    {
                        alpha = 1;
                        FlagCompletion();
                    }
                    break;

                case TransitionType.Enter:
                    alpha -= Force;
                    if (alpha < 0)
                    {
                        alpha = 0;
                        FlagCompletion();
                    }
                    break;
            }

            fadeColor = defaultColor * alpha;
            fade.SetColor(fadeColor);
        }

        protected override void DrawTransition(SpriteBatch spriteBatch)
        {
            fade.Draw(spriteBatch, CameraType.Static);
        }
    }
}
