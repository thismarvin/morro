using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public enum CameraType
    {
        /// <summary>
        /// The static camera will never move, and is used for drawing anything that must always be visible on the screen (e.g. menus, transitions, etc.).
        /// </summary>
        Static,

        /// <summary>
        /// The TopLeftAlign camera will also never move, but if WideScreenSupported is true, anything drawn will become top-left aligned to use the extra window space.
        /// </summary>
        TopLeftAlign,
        /// <summary>
        /// The RightAlign camera will also never move, but if WideScreenSupported is true, anything drawn will become top-right aligned to use the extra window space.
        /// </summary>
        TopRightAlign,
    }

    class CameraManager
    {
        private static Dictionary<string, Camera> cameras;

        internal static void Initialize()
        {
            cameras = new Dictionary<string, Camera>();

            RegisterCamera(new Camera("Static"));
            RegisterCamera(new Camera("TopLeftAlign"));
            RegisterCamera(new Camera("TopRightAlign"));

            WindowManager.WindowChanged += HandleWindowChange;
        }

        public static void RegisterCamera(Camera camera)
        {
            if (cameras.ContainsKey(camera.Name))
                throw new MorroException("A Camera with that name already exists; try a different name.", new ArgumentException("An item with the same key has already been added."));

            cameras.Add(camera.Name, camera);
        }

        public static Camera GetCamera(string name)
        {
            string formattedName = FormatCameraName(name);
            if (!cameras.ContainsKey(formattedName))
                throw new Exception("A camera with that name does not exist.", new KeyNotFoundException());

            return cameras[formattedName];
        }

        public static Camera GetCamera(CameraType cameraType)
        {
            return GetCamera(cameraType.ToString());
        }

        internal static string FormatCameraName(string name)
        {
            return name.ToLowerInvariant();
        }

        private static void HandleWindowChange(object sender, EventArgs e)
        {
            ResetCameras();
        }

        private static void ResetCameras()
        {
            foreach (KeyValuePair<string, Camera> entry in cameras)
            {
                entry.Value.Reset();
            }
        }

        private static void ManageManagedCameras()
        {
            if (WindowManager.WideScreenSupported)
            {
                GetCamera("TopLeftAlign").SetTopLeft(WindowManager.PillarBox, WindowManager.LetterBox);
                GetCamera("TopRightAlign").SetTopLeft(-WindowManager.PillarBox, -WindowManager.LetterBox);
            }
            else
            {
                GetCamera("TopLeftAlign").SetTopLeft(0, 0);
                GetCamera("TopRightAlign").SetTopLeft(0, 0);
            }

            GetCamera("Static").SetTopLeft(0, 0);
        }

        private static void UpdateCameras()
        {
            foreach (KeyValuePair<string, Camera> entry in cameras)
            {
                entry.Value.Update();
            }
        }

        public static void Update()
        {
            ManageManagedCameras();
            UpdateCameras();
        }
    }
}
