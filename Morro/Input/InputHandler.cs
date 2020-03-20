using Microsoft.Xna.Framework;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Input
{
    class InputHandler
    {
        public PlayerIndex PlayerIndex { get; private set; }
        public string Profile { get; private set; }        

        private InputProfile inputProfile;
        private readonly MGamePad gamePad;

        public InputHandler(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            inputProfile = InputManager.GetProfile("Basic");
            gamePad = new MGamePad(PlayerIndex);
        }

        public void LoadProfile(string profile)
        {
            if (Profile == profile.ToUpper(CultureInfo.InvariantCulture))
                return;

            Profile = profile.ToUpper(CultureInfo.InvariantCulture);
            inputProfile = InputManager.GetProfile(Profile);
        }

        public bool Pressing(string name)
        {
            InputMapping inputMapping = inputProfile.GetMapping(name);

            if (PlayerIndex == PlayerIndex.One)
            {
                for (int i = 0; i < inputMapping.Keys.Length; i++)
                {
                    if (MKeyboard.Pressing(inputMapping.Keys[i]))
                    {
                        return true;
                    }
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
            InputMapping inputMapping = inputProfile.GetMapping(name);

            if (PlayerIndex == PlayerIndex.One)
            {
                for (int i = 0; i < inputMapping.Keys.Length; i++)
                {
                    if (MKeyboard.Pressed(inputMapping.Keys[i]))
                    {
                        return true;
                    }
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
