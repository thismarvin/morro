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
        private float alpha;
        private Color defaultColor;
        private Color fadeColor;
        private readonly Quad fade;

        public Fade(TransitionType type) : this(type, Color.Black, 1, 5)
        {

        }

        public Fade(TransitionType type, Color color, float speed, float jerk) : base(-BUFFER, -BUFFER, type)
        {
            defaultColor = color;
            this.speed = speed;
            this.jerk = jerk;

            alpha = Type == TransitionType.Enter ? 1 : 0;
            fadeColor = new Color(defaultColor, alpha);
            fade = new Quad(X, Y, Width, Height, fadeColor, VertexInformation.Dynamic);

            Reset();
        }

        public override void Reset()
        {
            base.Reset();

            alpha = Type == TransitionType.Enter ? 1 : 0;
            fadeColor = new Color(defaultColor, alpha);
            fade.SetColor(fadeColor);
        }

        public override void Update()
        {
            if (Done)
                return;

            CalculateForce();

            if (WindowManager.WideScreenSupported)
            {
                fade.SetBounds
                (
                    -WindowManager.PillarBox - BUFFER,
                    -WindowManager.LetterBox - BUFFER,
                    CameraManager.GetCamera(CameraType.Static).Bounds.Width + BUFFER * 2,
                    CameraManager.GetCamera(CameraType.Static).Bounds.Height + BUFFER * 2
                );
            }

            switch (Type)
            {
                case TransitionType.Exit:
                    alpha += velocity * Engine.DeltaTime;
                    if (alpha > 1)
                    {
                        alpha = 1;
                        lastDraw = true;
                    }
                    break;

                case TransitionType.Enter:
                    alpha -= velocity * Engine.DeltaTime;
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
            if (Done)
                return;

            fade.Draw(spriteBatch, CameraType.Static);

            if (lastDraw)
                Done = true;
        }
    }
}
