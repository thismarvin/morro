﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    public enum AnimationType
    {
        NoLoop,
        Loop
    }

    class AnimatedSprite : Sprite
    {
        public int Columns { get; private set; }
        public int TotalFrames { get; private set; }
        public int CurrentFrame { get; private set; }       
        public float FrameDuration { get; private set; }
        public bool Finished { get; private set; }
        public bool AnimationPlaying { get; private set; }
        public AnimationType AnimationType { get; set; }       

        private SpriteType[] sprites;
        private Timer timer;        

        public AnimatedSprite(float x, float y, SpriteType sprite, AnimationType animationType, int totalFrames, int columns, float frameDuration) : this(x, y, sprite, animationType, totalFrames, columns, frameDuration, true)
        {

        }

        public AnimatedSprite(float x, float y, SpriteType sprite, AnimationType animationType, int totalFrames, int columns, float frameDuration, bool start) : base(x, y, sprite)
        {
            AnimationType = animationType;
            TotalFrames = totalFrames;
            Columns = columns;
            FrameDuration = frameDuration;
            AnimationPlaying = start;

            timer = new Timer(FrameDuration, AnimationPlaying);                     
        }

        public AnimatedSprite(float x, float y, SpriteType[] sprites, float frameDuration) : base(x, y, sprites[0])
        {
            this.sprites = sprites;
            TotalFrames = sprites.Length;
            Columns = TotalFrames;
            FrameDuration = frameDuration;
            timer = new Timer(FrameDuration);
            AnimationType = AnimationType.Loop;
        }

        public void PlayAnimation()
        {
            timer.Start();
        }

        public void PauseAnimation()
        {
            timer.Stop();
        }

        public void ResetAnimation()
        {
            timer.Reset();
            CurrentFrame = 0;
        }

        public SpriteType CurrentSprite()
        {
            return sprites[CurrentFrame];
        }

        public void SetCurrentFrame(int frame)
        {
            if (CurrentFrame == frame)
                return;

            CurrentFrame = frame;
            timer.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            if (Finished)
                return;           

            if (timer.Done())
            {
                switch (AnimationType)
                {
                    case AnimationType.Loop:
                        CurrentFrame = CurrentFrame >= TotalFrames - 1 ? 0 : ++CurrentFrame;
                        break;
                    case AnimationType.NoLoop:
                        CurrentFrame = CurrentFrame >= TotalFrames - 1 ? TotalFrames : ++CurrentFrame;
                        break;
                }
                timer.Reset();
            }

            Finished = AnimationType == AnimationType.NoLoop && CurrentFrame == TotalFrames;
            AnimationPlaying = !Finished && timer.Active;

            if (sprites == null)
                SetFrame(CurrentFrame, Columns);
            else
                SetSprite(CurrentSprite());
        }
    }
}