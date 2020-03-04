using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Morro.Core
{
    public enum OrientationType
    {
        Landscape,
        Portrait
    }

    static class WindowManager
    {
        public static int PixelWidth { get; private set; }
        public static int PixelHeight { get; private set; }
        public static float Scale { get; private set; }
        public static Rectangle Bounds { get; private set; }

        public static int DisplayWidth { get; private set; }
        public static int DisplayHeight { get; private set; }
        public static int DefaultWindowWidth { get; private set; }
        public static int DefaultWindowHeight { get; private set; }
        public static int WindowWidth { get => Engine.Graphics.PreferredBackBufferWidth; }
        public static int WindowHeight { get => Engine.Graphics.PreferredBackBufferHeight; }
        public static RenderTarget2D RenderTarget { get; private set; }
        public static OrientationType Orientation { get; private set; }
        public static string Title { get; set; }
        public static bool Fullscreen { get; private set; }
        public static bool IsWideScreen { get; private set; }
        public static bool WideScreenSupported { get; private set; }

        public static float FPS { get; private set; }

        private static readonly Queue<float> sampleFPS;

        public static float LetterBox { get; private set; }
        public static float PillarBox { get; private set; }

        private static int defaultPixelWidth;
        private static int defaultPixelHeight;

        private static MAABB[] boxing;
        private static readonly PolygonCollection polygonCollection;

        private static bool togglingFullscreen;

        public static EventHandler<EventArgs> WindowChanged { get; set; }
        private static void RaiseWindowChangedEvent()
        {
            WindowChanged?.Invoke(null, EventArgs.Empty);
        }

        static WindowManager()
        {
            sampleFPS = new Queue<float>();
            polygonCollection = new PolygonCollection();

            Engine.Instance.Window.ClientSizeChanged += HandleWindowResize;

            InitializeWindow();
            SetTitle("morroEngine");

            /// When a MonoGame Windows Project application is fullscreen and the game's window has not been moved since startup, 
            /// Microsoft.Xna.Framework.Input.Mouse.GetState().Y is offset unless the following line of code is included.
            Engine.Instance.Window.Position = Engine.Instance.Window.Position;
        }

        public static void SetPixelDimensions(int pixelWidth, int pixelHeight)
        {
            defaultPixelWidth = pixelWidth;
            defaultPixelHeight = pixelHeight;
            PixelWidth = defaultPixelWidth;
            PixelHeight = defaultPixelHeight;

            SetupOrientation();
            SetupBoxing();
            ResetScale();
        }

        public static void SetWindowDimensions(int defaultWindowWidth, int defaultWindowHeight)
        {
            DefaultWindowWidth = defaultWindowWidth;
            DefaultWindowHeight = defaultWindowHeight;
            DisplayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            DisplayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            // Set Screen Dimensions.
#if __IOS__ || __ANDROID__
            Engine.Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Engine.Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#else
            Engine.Graphics.PreferredBackBufferWidth = DefaultWindowWidth;
            Engine.Graphics.PreferredBackBufferHeight = DefaultWindowHeight;
#endif           
            Engine.Graphics.ApplyChanges();

            ResetScale();
        }

        public static void SetTitle(string title)
        {
            Title = title;
            Engine.Instance.Window.Title = Title;
        }

        public static void EnableVSync(bool enabled)
        {
            Engine.Instance.IsFixedTimeStep = false;

            if (enabled)
                Engine.Graphics.SynchronizeWithVerticalRetrace = true;
            else
                Engine.Graphics.SynchronizeWithVerticalRetrace = false;

            Engine.Graphics.ApplyChanges();
        }

        public static void EnableFullscreen(bool enabled)
        {
            if (enabled)
            {
                ToggleFullScreen();
            }
        }

        public static void EnableWideScreenSupport(bool enabled)
        {
            WideScreenSupported = enabled;
            IsWideScreen = GraphicsAdapter.DefaultAdapter.IsWideScreen;

            Engine.Graphics.ApplyChanges();
        }

        private static void InitializeWindow()
        {
            defaultPixelWidth = 320;
            defaultPixelHeight = 180;
            PixelWidth = defaultPixelWidth;
            PixelHeight = defaultPixelHeight;

            DefaultWindowWidth = PixelWidth * 2;
            DefaultWindowHeight = PixelHeight * 2;
            DisplayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            DisplayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Orientation = OrientationType.Landscape;
            Engine.Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            // Set Screen Dimensions.
#if __IOS__ || __ANDROID__
            Engine.Graphics.PreferredBackBufferWidth = DisplayWidth;
            Engine.Graphics.PreferredBackBufferHeight = DisplayHeight;
#else
            Engine.Graphics.PreferredBackBufferWidth = DefaultWindowWidth;
            Engine.Graphics.PreferredBackBufferHeight = DefaultWindowHeight;
#endif
            Engine.Instance.IsFixedTimeStep = false;
            Engine.Graphics.SynchronizeWithVerticalRetrace = true;

            Engine.Graphics.ApplyChanges();

            SetupBoxing();
            ResetScale();
        }

        private static void SetupOrientation()
        {
            Orientation = PixelWidth > PixelHeight ? OrientationType.Landscape : OrientationType.Portrait;

            // Set Supported Orientations.
            switch (Orientation)
            {
                case OrientationType.Landscape:
                    Engine.Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
                    break;
                case OrientationType.Portrait:
                    Engine.Graphics.SupportedOrientations = DisplayOrientation.Portrait;
                    break;
            }

            Engine.Graphics.ApplyChanges();
        }

        private static void SetupBoxing()
        {
            int buffer = 1000;

            boxing = new MAABB[]
            {
                new MAABB(-buffer, -buffer, defaultPixelWidth + buffer * 2, buffer) { Color = Color.Black },
                new MAABB(-buffer, defaultPixelHeight, defaultPixelWidth + buffer * 2, buffer) { Color = Color.Black },
                new MAABB(-buffer, -buffer, buffer, defaultPixelHeight + buffer * 2) { Color = Color.Black },
                new MAABB(defaultPixelWidth, -buffer, buffer, defaultPixelHeight + buffer * 2) { Color = Color.Black }
            };

            polygonCollection.SetCollection(boxing);
        }

        private static void CalculateScale()
        {
            Scale = CalculateZoom(WindowWidth, WindowHeight);
        }

        private static float CalculateZoom(int windowWidth, int windowHeight)
        {
            float zoom = 0;

            PixelWidth = defaultPixelWidth;
            PixelHeight = defaultPixelHeight;

            switch (Orientation)
            {
                case OrientationType.Landscape:
                    zoom = (float)windowHeight / PixelHeight;
                    // Check if letterboxing is required.
                    if (PixelWidth * zoom > windowWidth)
                    {
                        zoom = (float)windowWidth / PixelWidth;
                    }
                    Bounds = new Rectangle(0, 0, PixelWidth, PixelHeight);
                    break;

                case OrientationType.Portrait:
                    zoom = (float)windowWidth / PixelWidth;
                    // Check if letterboxing is required. ??? Im not sure if i really need this.
                    if (PixelHeight * zoom > windowHeight)
                    {
                        zoom = (float)windowHeight / PixelHeight;
                    }
                    Bounds = new Rectangle(0, 0, PixelWidth, PixelHeight);
                    break;
            }
            return zoom;
        }

        private static void CalculateBoxing()
        {
            LetterBox = (WindowHeight / Scale - defaultPixelHeight) / 2;
            PillarBox = (WindowWidth / Scale - defaultPixelWidth) / 2;
        }

        private static void UpdateRenderTarget()
        {
            RenderTarget?.Dispose();
            RenderTarget = new RenderTarget2D(Engine.Graphics.GraphicsDevice, WindowWidth, WindowHeight);
        }

        private static void ResetScale()
        {
            CalculateScale();
            CalculateBoxing();
            UpdateRenderTarget();

            RaiseWindowChangedEvent();
        }

        private static void ActivateFullScreenMode()
        {
            Engine.Graphics.PreferredBackBufferHeight = DisplayHeight;
            Engine.Graphics.PreferredBackBufferWidth = DisplayWidth;
        }

        private static void DeactivateFullScreen()
        {
            Engine.Graphics.PreferredBackBufferHeight = DefaultWindowHeight;
            Engine.Graphics.PreferredBackBufferWidth = DefaultWindowWidth;
        }

        private static void HandleWindowResize(object sender, EventArgs e)
        {
            if (togglingFullscreen)
                return;

            Engine.Graphics.PreferredBackBufferWidth = Math.Max(defaultPixelWidth, Engine.Instance.Window.ClientBounds.Width);
            Engine.Graphics.PreferredBackBufferHeight = Math.Max(defaultPixelHeight, Engine.Instance.Window.ClientBounds.Height);
            Engine.Graphics.ApplyChanges();

            ResetScale();
        }

        private static void ToggleFullScreen()
        {
            togglingFullscreen = true;

            if (Fullscreen)
            {
                DeactivateFullScreen();
            }
            else
            {
                ActivateFullScreenMode();
            }

            Engine.Graphics.ToggleFullScreen();
            Engine.Graphics.ApplyChanges();

            Fullscreen = !Fullscreen;
            togglingFullscreen = false;

            ResetScale();
        }

        private static void CalculateFPS()
        {
            if (Engine.DeltaTime != 0)
            {
                sampleFPS.Enqueue(1 / Engine.DeltaTime);
            }

            if (sampleFPS.Count == 100)
            {
                FPS = sampleFPS.Average(i => i);
                sampleFPS.Dequeue();
            }
        }

        private static void UpdateInput()
        {
            if (Input.Keyboard.Pressed(Keys.F11))
            {
                ToggleFullScreen();
            }

#if !__IOS__ && !__TVOS__
            if (Input.Keyboard.Pressed(Keys.Escape))
            {
                Engine.Instance.Exit();
            }
#endif
        }

        internal static void Update()
        {
            UpdateInput();
            CalculateFPS();
        }

        internal static void Draw()
        {
            if (!WideScreenSupported)
            {
                polygonCollection.Draw(CameraManager.GetCamera(CameraType.Static));
            }
        }
    }
}
