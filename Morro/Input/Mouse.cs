using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Input
{
    static class Mouse
    {
        public static Vector2 DynamicLocation { get { return dynamicLocation; } }
        public static Vector2 StaticLocation { get { return staticLocation; } }
        public static Core.Rectangle DynamicBounds { get; private set; }
        public static Core.Rectangle StaticBounds { get; private set; }

        private static MouseState previousMouseState;
        private static MouseState currentMouseState;
        private static Vector2 dynamicLocation;
        private static Vector2 staticLocation;
        private static bool isBeingUpdated;

        private static void VerifyUpdateIsCalled()
        {
            if (!isBeingUpdated)
            {
                throw new Exception("Make sure to call Input.Mouse.Update() in Engine.cs before you use any of the built in methods.");
            }
        }

        public static void Update()
        {
            isBeingUpdated = true;
            previousMouseState = currentMouseState;
            currentMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            dynamicLocation.X = currentMouseState.X / CameraManager.GetCamera(CameraType.Dynamic).Zoom + CameraManager.GetCamera(CameraType.Dynamic).TopLeft.X - WindowManager.PillarBox;
            dynamicLocation.Y = currentMouseState.Y / CameraManager.GetCamera(CameraType.Dynamic).Zoom + CameraManager.GetCamera(CameraType.Dynamic).TopLeft.Y - WindowManager.LetterBox;

            staticLocation.X = currentMouseState.X / CameraManager.GetCamera(CameraType.Static).Zoom - WindowManager.PillarBox;
            staticLocation.Y = currentMouseState.Y / CameraManager.GetCamera(CameraType.Static).Zoom - WindowManager.LetterBox;

            DynamicBounds = new Core.Rectangle(dynamicLocation.X, dynamicLocation.Y, 1, 1);
            StaticBounds = new Core.Rectangle(staticLocation.X, staticLocation.Y, 1, 1);
        }

        public static bool PressedLeftClick()
        {
            VerifyUpdateIsCalled();
            return previousMouseState.LeftButton != ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool PressingLeftClick()
        {
            VerifyUpdateIsCalled();
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool PressedRightClick()
        {
            VerifyUpdateIsCalled();
            return previousMouseState.RightButton != ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool PressingRightClick()
        {
            VerifyUpdateIsCalled();
            return currentMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool PressedMiddleButton()
        {
            VerifyUpdateIsCalled();
            return previousMouseState.MiddleButton != ButtonState.Pressed && currentMouseState.MiddleButton == ButtonState.Pressed;
        }

        public static bool PressingMiddleButton()
        {
            VerifyUpdateIsCalled();
            return currentMouseState.MiddleButton == ButtonState.Pressed;
        }

        public static bool ScrollingUp()
        {
            VerifyUpdateIsCalled();
            return currentMouseState.ScrollWheelValue > previousMouseState.ScrollWheelValue;
        }

        public static bool ScrollingDown()
        {
            VerifyUpdateIsCalled();
            return currentMouseState.ScrollWheelValue < previousMouseState.ScrollWheelValue;
        }
    }
}
