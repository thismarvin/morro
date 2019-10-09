﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Input
{
    class GamePad
    {
        public PlayerIndex PlayerIndex { get; private set; } 

        private GamePadState previousGamePadState;
        private GamePadState currentGamePadState;
        private bool isBeingUpdated;

        public GamePad(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        private void VerifyUpdateIsCalled()
        {
            if (!isBeingUpdated)
            {
                throw new Exception("Make sure to call your GamePad's Update() method before you use any of the built in methods.");
            }
        }

        public void Update()
        {
            isBeingUpdated = true;
            previousGamePadState = currentGamePadState;
            currentGamePadState = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex);
        }

        public bool IsConnected()
        {
            VerifyUpdateIsCalled();
            return currentGamePadState.IsConnected;
        }

        public bool PressedButton(Buttons button)
        {
            VerifyUpdateIsCalled();
            return !previousGamePadState.IsButtonDown(button) && currentGamePadState.IsButtonDown(button);
        }

        public bool PressingButton(Buttons button)
        {
            VerifyUpdateIsCalled();
            return currentGamePadState.IsButtonDown(button);
        }
    }
}