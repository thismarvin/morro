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

        public static void Initialize(int pixelWidth, int pixelHeight, int windowWidth, int windowHeight, OrientationType orientation, string title, bool enableVSync, bool startFullScreen, bool supportWideScreen)
        {
            SetupPixelScene(pixelWidth, pixelHeight);
            SetupWindow(windowWidth, windowHeight, orientation);
            SetupTitle(title);
            EnableVSync(enableVSync);
            EnableFullscreen(startFullScreen);
            SetupWideScreenSupport(supportWideScreen);
            SetupBoxing();

            Engine.Graphics.ApplyChanges();

            CalculateScale();
            CalculateBoxing();
            UpdateRenderTarget();

            sampleFPS = new Queue<float>();

            Engine.Instance.Window.ClientSizeChanged += HandleWindowResize;
        }

        private static void SetupPixelScene(int pixelWidth, int pixelHeight)
        {
            defaultPixelWidth = pixelWidth;
            defaultPixelHeight = pixelHeight;
            PixelWidth = defaultPixelWidth;
            PixelHeight = defaultPixelHeight;
        }

        private static void SetupWindow(int defaultWindowWidth, int defaultWindowHeight, OrientationType orientation)
        {
            DefaultWindowWidth = defaultWindowWidth;
            DefaultWindowHeight = defaultWindowHeight;
            DisplayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            DisplayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Orientation = orientation;

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

            // Make sure pixel dimensions are in line with the game's orientation.
            if (Orientation == OrientationType.Landscape && PixelHeight > PixelWidth)
            {
                throw new Exception("When the Orientation is set to Landscape, PixelWidth must be greater than PixelHeight.");
            }
            else if (Orientation == OrientationType.Portrait && PixelWidth > PixelHeight)
            {
                throw new Exception("When the Orientation is set to Portrait, PixelHeight must be greater than PixelWidth.");
            }

            // Set Screen Dimensions.
#if __IOS__ || __ANDROID__
            Engine.Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Engine.Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
#else
            Engine.Graphics.PreferredBackBufferWidth = DefaultWindowWidth;
            Engine.Graphics.PreferredBackBufferHeight = DefaultWindowHeight;
#endif           
        }

        private static void SetupTitle(string title)
        {
            Title = title;
            Engine.Instance.Window.Title = Title;
        }

        private static void EnableVSync(bool vsync)
        {
            Engine.Instance.IsFixedTimeStep = false;

            if (vsync)
                Engine.Graphics.SynchronizeWithVerticalRetrace = true;
            else
                Engine.Graphics.SynchronizeWithVerticalRetrace = false;
        }

        private static void EnableFullscreen(bool enableFullScreen)
        {
            if (enableFullScreen)
            {
                ToggleFullScreen();
            }
        }

        private static void SetupWideScreenSupport(bool supportWideScreen)
        {
            WideScreenSupported = supportWideScreen;
            IsWideScreen = GraphicsAdapter.DefaultAdapter.IsWideScreen;
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

        private static void CalculateFPS(GameTime gameTime)
        {
            if ((float)gameTime.ElapsedGameTime.TotalSeconds != 0)
            {
                sampleFPS.Enqueue(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
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
        }

        public static void Update(GameTime gameTime)
        {
            UpdateInput();
            CalculateFPS(gameTime);
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
