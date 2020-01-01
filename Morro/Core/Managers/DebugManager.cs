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
    /// Maybe make this more Modular?
    /// For example you can have a DebugFlag class or like a DebugEntry class.
    static class DebugManager
    {
        public static bool Debugging { get; private set; }
        public static bool ShowWireFrame { get; private set; }
        public static bool ShowDebugLayer { get; private set; }

        private static Dictionary<string, DebugEntry> debugEntries;

        internal static void Initialize()
        {
            debugEntries = new Dictionary<string, DebugEntry>();

            AddDebugEntry(new DebugEntry("FPS", "{0} FPS"));
            AddDebugEntry(new DebugEntry("Scene", "SCENE: {0}"));
            AddDebugEntry(new DebugEntry("Entities", "ENTITIES: {0}"));
        }

        internal static string FormatName(string name)
        {
            return name.ToLowerInvariant();
        }

        #region Handle DebugEntries
        public static void AddDebugEntry(DebugEntry debugEntry)
        {
            if (debugEntries.ContainsKey(debugEntry.Name))
                throw new MorroException("A DebugEntry with that name already exists; try a different name.", new ArgumentException("An item with the same key has already been added."));

            debugEntries.Add(debugEntry.Name, debugEntry);
        }

        public static DebugEntry GetDebugEntry(string name)
        {
            string formattedName = FormatName(name);
            VerifyDebugEntryExists(formattedName);

            return debugEntries[formattedName];
        }

        public static void RemoveDebugEntry(string name)
        {
            string formattedName = FormatName(name);
            VerifyDebugEntryExists(formattedName);

            debugEntries[formattedName].Dispose();
            debugEntries.Remove(formattedName);
        }

        private static void VerifyDebugEntryExists(string name)
        {
            if (!debugEntries.ContainsKey(name))
                throw new Exception("A debug entry with that name does not exist.", new KeyNotFoundException());
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

            foreach (KeyValuePair<string, DebugEntry> entry in debugEntries)
            {
                entry.Value.Draw(spriteBatch, CameraType.TopLeftAlign);
            }
        }

        public static void Update()
        {
            UpdateInput();
            UpdateInfo();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            DrawDebugLayer(spriteBatch);
            DrawDebugEntries(spriteBatch);
        }
    }
}
