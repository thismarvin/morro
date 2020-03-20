using Microsoft.Xna.Framework.Input;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Input
{
    static class MKeyboard
    {        
        private static KeyboardState previousKeyState;
        private static KeyboardState currentKeyState;
        private static bool isBeingUpdated;

        public static bool Pressed(Keys key)
        {
            VerifyUpdateIsCalled();
            
            if (!previousKeyState.IsKeyDown(key) && currentKeyState.IsKeyDown(key))
            {
                InputManager.InputMode = InputMode.Keyboard;
                return true;
            }

            return false;
        }

        public static bool Pressing(Keys key)
        {            
            VerifyUpdateIsCalled();

            if (currentKeyState.IsKeyDown(key))
            {
                InputManager.InputMode = InputMode.Keyboard;
                return true;
            }

            return false;
        }

        private static void VerifyUpdateIsCalled()
        {
            if (!isBeingUpdated)
            {
                throw new MorroException("Make sure to call Input.Keyboard.Update() in Engine.cs before you use any of the built in methods.", new MethodExpectedException());
            }
        }

        internal static void Update()
        {
            isBeingUpdated = true;
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
        }
    }
}
