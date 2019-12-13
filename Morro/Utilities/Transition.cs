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

    abstract class Transition
    {
        public bool Started { get; private set; }
        public bool Done { get; protected set; }
        protected TransitionType Type { get; private set; }

        protected bool setup;
        protected bool lastDraw;

        public float Force { get; private set; }
        protected float velocity;
        protected float acceleration;

        public Transition(TransitionType type, float velocity, float acceleration)
        {
            Type = type;
            this.velocity = velocity;
            this.acceleration = acceleration;            
        }

        public virtual void Reset()
        {
            Started = false;
            Done = false;
            setup = false;
            lastDraw = false;

            Force = 0;
            velocity = 0;
            acceleration = 0;
        }

        public void Begin()
        {
            Started = true;
        }

        protected void CalculateForce()
        {
            Force += velocity * Engine.DeltaTime + 0.5f * acceleration * Engine.DeltaTime * Engine.DeltaTime;
            velocity += acceleration * Engine.DeltaTime;
        }

        public abstract void Update();

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
