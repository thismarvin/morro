using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.Graphics;
using Morro.Utilities;
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
        public static int WindowWidth { get { return Engine.Graphics.PreferredBackBufferWidth; } }
        public static int WindowHeight { get { return Engine.Graphics.PreferredBackBufferHeight; } }
        public static RenderTarget2D RenderTarget { get; private set; }
        public static OrientationType Orientation { get; private set; }
        public static string Title { get; set; }
        public static bool Fullscreen { get; private set; }
        public static bool IsWideScreen { get; private set; }
        public static bool WideScreenSupported { get; private set; }

        public static float FPS { get; private set; }

        private static Queue<float> sampleFPS;

        public static float LetterBox { get; private set; }
        public static float PillarBox { get; private set; }

        private static int defaultPixelWidth;
        private static int defaultPixelHeight;
        private static Quad topLetterBox;
        private static Quad bottomLetterBox;
        private static Quad leftPillarBox;
        private static Quad rightPillarBox;
        private static bool togglingFullscreen;

        public static EventHandler<EventArgs> WindowChanged { get; set; }
        private static void RaiseWindowChangedEvent()
        {
            WindowChanged?.Invoke(null, EventArgs.Empty);
        }

        internal static void Initialize()
        {
            sampleFPS = new Queue<float>();
            Engine.Instance.Window.ClientSizeChanged += HandleWindowResize;

            InitializeWindow();            

            SetTitle("morroEngine");
            EnableVSync(true);
            EnableFullscreen(false);
            SetupWideScreenSupport(true);
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

        public static void SetupWideScreenSupport(bool supportWideScreen)
        {
            WideScreenSupported = supportWideScreen;
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
            topLetterBox = new Quad(-buffer, -buffer, defaultPixelWidth + buffer * 2, buffer, Color.Black, VertexInformation.Static);
            bottomLetterBox = new Quad(-buffer, defaultPixelHeight, defaultPixelWidth + buffer * 2, buffer, Color.Black, VertexInformation.Static);
            leftPillarBox = new Quad(-buffer, -buffer, buffer, defaultPixelHeight + buffer * 2, Color.Black, VertexInformation.Static);
            rightPillarBox = new Quad(defaultPixelWidth, -buffer, buffer, defaultPixelHeight + buffer * 2, Color.Black, VertexInformation.Static);
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
                    else if (PixelWidth * zoom < windowWidth)
                    {
                        // Disable letterboxing if WideScreenSupport is enabled.
                        if (WideScreenSupported)
                        {
                            PixelWidth = (int)((windowWidth - PixelWidth * zoom) / zoom) + PixelWidth;
                        }
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
                DeactivateFullScreen();
            else
                ActivateFullScreenMode();

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

        public static void Update()
        {
            UpdateInput();
            CalculateFPS();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            switch (Orientation)
            {
                case OrientationType.Landscape:
                    topLetterBox.Draw(spriteBatch, CameraType.Static);
                    bottomLetterBox.Draw(spriteBatch, CameraType.Static);

                    if (!WideScreenSupported)
                    {
                        leftPillarBox.Draw(spriteBatch, CameraType.Static);
                        rightPillarBox.Draw(spriteBatch, CameraType.Static);
                    }
                    break;

                case OrientationType.Portrait:
                    leftPillarBox.Draw(spriteBatch, CameraType.Static);
                    rightPillarBox.Draw(spriteBatch, CameraType.Static);

                    if (!WideScreenSupported)
                    {
                        topLetterBox.Draw(spriteBatch, CameraType.Static);
                        bottomLetterBox.Draw(spriteBatch, CameraType.Static);
                    }
                    break;
            }
        }
    }
}
