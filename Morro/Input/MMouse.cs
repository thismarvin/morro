using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Input
{
    static class MMouse
    {
        public static Vector2 SceneLocation { get => sceneLocation; }
        public static Vector2 WindowLocation { get => windowLocation; }
        public static RectangleF DynamicBounds { get; private set; }
        public static RectangleF StaticBounds { get; private set; }

        private static MouseState previousMouseState;
        private static MouseState currentMouseState;
        private static Vector2 sceneLocation;
        private static Vector2 windowLocation;
        private static bool isBeingUpdated;

        private static void VerifyUpdateIsCalled()
        {
            if (!isBeingUpdated)
            {
                throw new MorroException("Make sure to call Input.Mouse.Update() in Engine.cs before you use any of the built in methods.", new MethodExpectedException());
            }
        }

        public static void Update()
        {
            isBeingUpdated = true;
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            if (SceneManager.CurrentScene != null)
            {
                sceneLocation.X = currentMouseState.X / SceneManager.CurrentScene.Camera.Zoom + SceneManager.CurrentScene.Camera.TopLeft.X - WindowManager.PillarBox;
                sceneLocation.Y = currentMouseState.Y / SceneManager.CurrentScene.Camera.Zoom + SceneManager.CurrentScene.Camera.TopLeft.Y - WindowManager.LetterBox;
            }

            windowLocation.X = currentMouseState.X / CameraManager.GetCamera(CameraType.Static).Zoom - WindowManager.PillarBox;
            windowLocation.Y = currentMouseState.Y / CameraManager.GetCamera(CameraType.Static).Zoom - WindowManager.LetterBox;

            DynamicBounds = new RectangleF(sceneLocation.X, sceneLocation.Y, 1, 1);
            StaticBounds = new RectangleF(windowLocation.X, windowLocation.Y, 1, 1);
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
