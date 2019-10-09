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
        private Quad fade;

        public Fade(TransitionType type) : this(type, Color.Black, 1, 2)
        {

        }

        public Fade(TransitionType type, Color color) : this(type, color, 1, 2)
        {

        }

        public Fade(TransitionType type, Color color, float speed, float jerk) : base(-BUFFER, -BUFFER, type)
        {
            defaultColor = color;
            this.speed = speed;
            this.jerk = jerk;

            Reset();
        }

        public override void Reset()
        {
            base.Reset();

            alpha = Type == TransitionType.Enter ? (byte)255 : (byte)0;
            fadeColor = new Color(defaultColor, alpha);
            fade = new Quad(X, Y, Width, Height, fadeColor, VertexInformation.Dynamic);
        }

        public override void Update()
        {
            if (!InProgress)
                return;

            CalculateForce();

            switch (Type)
            {
                case TransitionType.Exit:
                    if (alpha + velocity < 1)
                    {
                        alpha += velocity;
                    }
                    else
                    {
                        alpha = 1;
                        lastDraw = true;
                    }
                    break;

                case TransitionType.Enter:
                    if (alpha - velocity > 0)
                    {
                        alpha -= velocity;
                    }
                    else
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
            if (!InProgress)
                return;

            fade.Draw(spriteBatch, CameraType.Static);

            if (lastDraw)
                Done = true;
        }
    }
}
