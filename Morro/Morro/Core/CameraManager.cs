using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public enum CameraType
    {
        /// <summary>
        /// Anything drawn with a dynamic camera will move inversely to the camera's top left. This effect will create the illusion of movement.
        /// </summary>
        Dynamic,
        /// <summary>
        /// The static camera will never move, and is used for drawing anything that must always be visible on the screen (e.g. menus, transitions, etc.).
        /// </summary>
        Static,
        /// <summary>
        /// The LeftJustified camera will also never move, but if WideScreenSupported is true, anything drawn will become left justified to use the extra window space.
        /// </summary>
        LeftJustified,
        /// <summary>
        /// The RightJustified camera will also never move, but if WideScreenSupported is true, anything drawn will become right justified to use the extra window space.
        /// </summary>
        RightJustified,
    }

    class CameraManager
    {
        private static List<Camera> cameras;

        public static void Initialize()
        {
            cameras = new List<Camera>()
            {
                new Camera(CameraType.Dynamic),
                new Camera(CameraType.Static),
                new Camera(CameraType.LeftJustified),
                new Camera(CameraType.RightJustified),
            };

            WindowManager.WindowChanged += HandleWindowChange;
        }

        public static Camera GetCamera(CameraType type)
        {
            foreach (Camera camera in cameras)
            {
                if (camera.CameraType == type)
                {
                    return camera;
                }
            }
            return null;
        }

        private static void HandleWindowChange(object sender, EventArgs e)
        {
            ResetCameras();
        }

        private static void ResetCameras()
        {
            foreach (Camera camera in cameras)
            {
                camera.Reset();
            }
        }

        public static void Update()
        {
            if (WindowManager.WideScreenSupported)
            {
                GetCamera(CameraType.LeftJustified).SetTopLeft(WindowManager.PillarBox, 0);
                GetCamera(CameraType.RightJustified).SetTopLeft(-WindowManager.PillarBox, 0);
            }
            else
            {
                GetCamera(CameraType.LeftJustified).SetTopLeft(0, 0);
                GetCamera(CameraType.RightJustified).SetTopLeft(0, 0);
            }

            GetCamera(CameraType.Static).SetTopLeft(0, 0);

            foreach (Camera camera in cameras)
            {
                camera.Update();
            }
        }
    }
}
