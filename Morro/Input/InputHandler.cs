﻿using Microsoft.Xna.Framework;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Input
{
    class InputHandler
    {
        public PlayerIndex PlayerIndex { get; private set; }

        private readonly InputProfile inputProfile;
        private readonly GamePad gamePad;

        public InputHandler(string profile, PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            inputProfile = InputManager.Profiles[profile];
            gamePad = new GamePad(PlayerIndex);
        }

        public bool Pressing(string name)
        {
            InputMapping inputMapping = inputProfile.InputMappings[name.ToUpper()];

            for (int i = 0; i < inputMapping.Keys.Length; i++)
            {
                if (Keyboard.Pressing(inputMapping.Keys[i]))
                {
                    return true;
                }
            }

            if (gamePad.IsConnected())
            {
                for (int i = 0; i < inputMapping.Buttons.Length; i++)
                {
                    if (gamePad.Pressing(inputMapping.Buttons[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Pressed(string name)
        {
            InputMapping inputMapping = inputProfile.InputMappings[name.ToUpper()];

            for (int i = 0; i < inputMapping.Keys.Length; i++)
            {
                if (Keyboard.Pressed(inputMapping.Keys[i]))
                {
                    return true;
                }
            }

            if (gamePad.IsConnected())
            {
                for (int i = 0; i < inputMapping.Buttons.Length; i++)
                {
                    if (gamePad.Pressed(inputMapping.Buttons[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetVibration(float leftMotor, float rightMotor)
        {
            gamePad.Vibrate(leftMotor, rightMotor);
        }

        public void ResetVibration()
        {
            gamePad.Vibrate(0, 0);
        }

        public void Update()
        {
            gamePad.Update();
        }
    }
}
