using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
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

        public void Vibrate(float leftMotor, float rightMotor)
        {
            if (InputManager.InputMode == InputMode.Controller)
                Microsoft.Xna.Framework.Input.GamePad.SetVibration(PlayerIndex, leftMotor, rightMotor);
            else
                Microsoft.Xna.Framework.Input.GamePad.SetVibration(PlayerIndex, 0, 0);
        }

        public bool Pressed(Buttons button)
        {
            VerifyUpdateIsCalled();

            if (!previousGamePadState.IsButtonDown(button) && currentGamePadState.IsButtonDown(button))
            {
                InputManager.SetInputMode(InputMode.Controller);
                return true;
            }
            return false;
        }

        public bool Pressing(Buttons button)
        {
            VerifyUpdateIsCalled();

            if (currentGamePadState.IsButtonDown(button))
            {
                InputManager.SetInputMode(InputMode.Controller);
                return true;
            }
            return false;
        }
    }
}
