using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Input
{
    static class Keyboard
    {        
        private static KeyboardState previousKeyState;
        private static KeyboardState currentKeyState;
        private static bool isBeingUpdated;

        private static void VerifyUpdateIsCalled()
        {
            if (!isBeingUpdated)
            {
                throw new Exception("Make sure to call Input.Keyboard.Update() in Engine.cs before you use any of the built in methods.");
            }
        }

        public static void Update()
        {
            isBeingUpdated = true;
            previousKeyState = currentKeyState;
            currentKeyState = Microsoft.Xna.Framework.Input.Keyboard.GetState();            
        }

        public static bool Pressed(Keys key)
        {
            VerifyUpdateIsCalled();
            return !previousKeyState.IsKeyDown(key) && currentKeyState.IsKeyDown(key);
        }

        public static bool Pressing(Keys key)
        {
            VerifyUpdateIsCalled();
            return currentKeyState.IsKeyDown(key);
        }
    }
}
