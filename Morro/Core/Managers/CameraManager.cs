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
        /// <summary>
        /// The TopJustified camera will also never move, but if WideScreenSupported is true, anything drawn will become top justified to use the extra window space.
        /// </summary>
        TopJustified,
        /// <summary>
        /// The BottomJustified camera will also never move, but if WideScreenSupported is true, anything drawn will become bottom justified to use the extra window space.
        /// </summary>
        BottomJustified,
    }

    class CameraManager
    {
        private static Dictionary<CameraType, Camera> cameras;

        public static void Initialize()
        {
            cameras = new Dictionary<CameraType, Camera>
            {
                { CameraType.Static, new Camera(CameraType.Static) },
                { CameraType.Dynamic, new Camera(CameraType.Dynamic) },

                { CameraType.LeftJustified, new Camera(CameraType.LeftJustified) },
                { CameraType.RightJustified, new Camera(CameraType.RightJustified) },

                { CameraType.TopJustified, new Camera(CameraType.TopJustified) },
                { CameraType.BottomJustified, new Camera(CameraType.BottomJustified) }
            };

            WindowManager.WindowChanged += HandleWindowChange;
        }

        public static Camera GetCamera(CameraType type)
        {
            return cameras[type];
        }

        private static void HandleWindowChange(object sender, EventArgs e)
        {
            ResetCameras();
        }

        private static void ResetCameras()
        {
            foreach (KeyValuePair<CameraType, Camera> entry in cameras)
            {
                entry.Value.Reset();
            }
        }

        public static void Update()
        {
            if (WindowManager.WideScreenSupported)
            {
                GetCamera(CameraType.LeftJustified).SetTopLeft(WindowManager.PillarBox, 0);
                GetCamera(CameraType.RightJustified).SetTopLeft(-WindowManager.PillarBox, 0);
                GetCamera(CameraType.TopJustified).SetTopLeft(0, WindowManager.LetterBox);
                GetCamera(CameraType.BottomJustified).SetTopLeft(0, -WindowManager.LetterBox);
            }
            else
            {
                GetCamera(CameraType.LeftJustified).SetTopLeft(0, 0);
                GetCamera(CameraType.RightJustified).SetTopLeft(0, 0);
                GetCamera(CameraType.TopJustified).SetTopLeft(0, 0);
                GetCamera(CameraType.BottomJustified).SetTopLeft(0, 0);
            }

            GetCamera(CameraType.Static).SetTopLeft(0, 0);

            foreach (KeyValuePair<CameraType, Camera> entry in cameras)
            {
                entry.Value.Update();
            }
        }
    }
}
