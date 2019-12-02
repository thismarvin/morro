using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Utilities
{
    public enum TransitionType
    {
        Enter,
        Exit
    }

    abstract class Transition : MonoObject
    {
        protected const int BUFFER = 100;
        public bool Started { get; private set; }
        public bool Done { get; protected set; }
        protected TransitionType Type { get; private set; }

        protected bool lastDraw;

        protected float speed; // more like initial speed
        protected float jerk;

        protected float velocity;
        protected float acceleration;

        private bool initialSpeedSet;

        public Transition(TransitionType type) : this(WindowManager.PixelWidth / 2, WindowManager.PixelHeight / 2, type)
        {

        }

        public Transition(float x, float y, TransitionType type) : base(x, y, WindowManager.PixelWidth + BUFFER * 2, WindowManager.PixelHeight + BUFFER * 2)
        {
            Type = type;
        }

        public virtual void Reset()
        {
            Started = false;
            Done = false;
            lastDraw = false;
            initialSpeedSet = false;

            velocity = 0;
            acceleration = 0;
        }

        public void Start()
        {
            Started = true;
        }

        protected void CalculateForce()
        {
            if (!initialSpeedSet)
            {
                velocity = speed;
                initialSpeedSet = true;
            }

            velocity += 0.5f * jerk * Engine.DeltaTime * Engine.DeltaTime + acceleration * Engine.DeltaTime;
            acceleration += jerk * Engine.DeltaTime;
        }

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
