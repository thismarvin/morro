using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.Debug;
using Morro.ECS;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Core
{
    /// Maybe make this more Modular?
    /// For example you can have a DebugFlag class or like a DebugEntry class.
    static class DebugManager
    {
        public static bool Debugging { get; private set; }
        public static bool ShowWireFrame { get; private set; }
        public static bool ShowDebugLayer { get; private set; }

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
                ShowDebugLayer = !ShowDebugLayer;
            }
        }

        private static void UpdateInfo()
        {
            if (!Debugging)
                return;

            FPS.SetText(string.Format(CultureInfo.InvariantCulture, "{0} FPS", Math.Round(WindowManager.FPS).ToString(CultureInfo.InvariantCulture)));
            currentScene.SetText(string.Format(CultureInfo.InvariantCulture, "CURRENT SCENE: {0}", SceneManager.CurrentScene.Name.ToString()));
            totalEntities.SetText(string.Format(CultureInfo.InvariantCulture, "TOTAL ENTITIES: {0}", SceneManager.CurrentScene.Entities.Count.ToString(CultureInfo.InvariantCulture)));
        }

        private static void DrawDebugLayer(SpriteBatch spriteBatch)
        {
            if (!ShowDebugLayer)
                return;

            List<Entity> queryResult = SceneManager.CurrentScene.Query(SceneManager.CurrentScene.Camera.Bounds);
            for (int i = 0; i < queryResult.Count; i++)
            {
                if (queryResult[i] is IDebugable)
                {
                    ((IDebugable)queryResult[i]).Debug(spriteBatch, SceneManager.CurrentScene.Camera);
                }
            }
        }

        public static void Update()
        {
            UpdateInput();
            UpdateInfo();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (!Debugging)
                return;

            DrawDebugLayer(spriteBatch);

            FPS.Draw(spriteBatch, CameraType.TopLeftAlign);
            currentScene.Draw(spriteBatch, CameraType.TopLeftAlign);
            totalEntities.Draw(spriteBatch, CameraType.TopLeftAlign);
        }
    }
}
