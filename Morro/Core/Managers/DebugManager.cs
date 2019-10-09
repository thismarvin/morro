using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    /// Maybe make this more Modular?
    /// For example you can have a DebugFlag class or like a DebugEntry class.
    static class DebugManager
    {
        public static bool Debugging { get; private set; }
        public static bool ShowWireFrame { get; private set; }
        public static bool ShowBoundingBoxes { get; private set; }

        private static BitmapFont FPS;
        private static BitmapFont currentScene;
        private static BitmapFont totalEntities;

        public static void Initialize()
        {
            FPS = new BitmapFont(4, 4, "FPS", FontType.Probity);
            currentScene = new BitmapFont(4, 4 + 8 + 2, "CURRENT SCENE:", FontType.Probity);
            totalEntities = new BitmapFont(4, 4 + 8 + 2 + 8 + 2, "TOTAL ENTITIES:", FontType.Probity);
        }

        private static void UpdateInput()
        {
            if (Input.Keyboard.Pressed(Keys.F3))
            {
                Debugging = !Debugging;
            }

            if (!Debugging)
                return;

            if (Input.Keyboard.Pressing(Keys.LeftShift) && Input.Keyboard.Pressed(Keys.D1))
            {
                ShowWireFrame = !ShowWireFrame;
            }

            if (Input.Keyboard.Pressing(Keys.LeftShift) && Input.Keyboard.Pressed(Keys.D2))
            {
                ShowBoundingBoxes = !ShowBoundingBoxes;
            }
        }

        private static void UpdateInfo()
        {
            if (!Debugging)
                return;

            FPS.SetText(string.Format("{0} FPS", Math.Round(WindowManager.FPS).ToString()));
            currentScene.SetText(string.Format("CURRENT SCENE: {0}", SceneManager.CurrentScene.SceneType.ToString()));
            totalEntities.SetText(string.Format("TOTAL ENTITIES: {0}", SceneManager.CurrentScene.Entities.Count.ToString()));
        }

        public static void Update()
        {
            UpdateInput();
            UpdateInfo();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (Debugging)
            {
                FPS.Draw(spriteBatch, CameraType.LeftJustified);
                currentScene.Draw(spriteBatch, CameraType.LeftJustified);
                totalEntities.Draw(spriteBatch, CameraType.LeftJustified);
            }
        }
    }
}
