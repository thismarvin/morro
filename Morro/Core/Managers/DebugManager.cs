using Microsoft.Xna.Framework;
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
    static class DebugManager
    {
        public static bool Debugging { get; private set; }
        public static bool ShowWireFrame { get; private set; }
        public static bool ShowDebugLayer { get; private set; }

        private static readonly ResourceHandler<DebugEntry> debugEntries;

        static DebugManager()
        {
            debugEntries = new ResourceHandler<DebugEntry>();

            RegisterDebugEntry(new DebugEntry("FPS", "{0} FPS"));
            RegisterDebugEntry(new DebugEntry("Scene", "SCENE: {0}"));
            RegisterDebugEntry(new DebugEntry("Entities", "ENTITIES: {0}"));
        }

        #region Handle DebugEntries
        /// <summary>
        /// Register a <see cref="DebugEntry"/> to be managed by Morro.
        /// </summary>
        /// <param name="debugEntry">The debug entry you want to register.</param>
        public static void RegisterDebugEntry(DebugEntry debugEntry)
        {
            debugEntries.Register(debugEntry.Name, debugEntry);
        }

        /// <summary>
        /// Get a <see cref="DebugEntry"/> that was previously registered.
        /// </summary>
        /// <param name="name">The name given to the debug entry that was previously registered.</param>
        /// <returns>The registered debug entry with the given name.</returns>
        public static DebugEntry GetDebugEntry(string name)
        {
            return debugEntries.Get(name);
        }

        /// <summary>
        /// Remove a registered <see cref="DebugEntry"/>.
        /// </summary>
        /// <param name="name">The name given to the debug entry that was previously registered.</param>
        public static void RemoveDebugEntry(string name)
        {
            debugEntries.Remove(name);
        }
        #endregion

        internal static Vector2 NextDebugEntryPosition()
        {
            int padding = 4;
            int textHeight = 8;
            int lineHeight = textHeight + 2;

            return new Vector2(padding, padding + debugEntries.Count * lineHeight);
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
            GetDebugEntry("FPS").SetInformation(Math.Round(WindowManager.FPS).ToString(CultureInfo.InvariantCulture));
            GetDebugEntry("Scene").SetInformation(SceneManager.CurrentScene.Name);
            //GetDebugEntry("Entities").SetInformation(SceneManager.CurrentScene.Entities.Count.ToString(CultureInfo.InvariantCulture));
        }

        private static void DrawDebugLayer(SpriteBatch spriteBatch)
        {
            if (!ShowDebugLayer)
                return;

            //List<Entity> queryResult = SceneManager.CurrentScene.Query(SceneManager.CurrentScene.Camera.Bounds);
            //for (int i = 0; i < queryResult.Count; i++)
            //{
            //    if (queryResult[i] is IDebugable)
            //    {
            //        ((IDebugable)queryResult[i]).Debug(spriteBatch, SceneManager.CurrentScene.Camera);
            //    }
            //}
        }

        private static void DrawDebugEntries(SpriteBatch spriteBatch)
        {
            if (!Debugging)
                return;

            foreach (DebugEntry debugEntry in debugEntries)
            {
                debugEntry.Draw(spriteBatch, CameraType.TopLeftAlign);
            }
        }

        internal static void Update()
        {
            UpdateInput();
            UpdateInfo();
        }

        internal static void Draw(SpriteBatch spriteBatch)
        {
            DrawDebugLayer(spriteBatch);
            DrawDebugEntries(spriteBatch);
        }
    }
}
