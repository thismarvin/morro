﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    abstract class Actor : Kinetic
    {
        public PlayerIndex PlayerIndex { get; private set; }

        public Actor(float x, float y, int width, int height, float moveSpeed, PlayerIndex playerIndex) : base(x, y, width, height, moveSpeed)
        {
            PlayerIndex = playerIndex;
        }

        protected abstract void UpdateInput();
    }
}