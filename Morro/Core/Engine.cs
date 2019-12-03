﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public class Engine : Game
    {
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static Engine Instance { get; private set; }
        public static RenderTarget2D RenderTarget { get { return WindowManager.RenderTarget; } }
        public static float DeltaTime { get; private set; }

        private SpriteBatch spriteBatch;

        public Engine()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Instance = this;

            IsMouseVisible = true;

            Window.AllowUserResizing = true;

            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            AssetManager.Initialize();
            AssetManager.LoadContent();

            GraphicsManager.Initialize();

            WindowManager.Initialize
            (
                pixelWidth: 320,
                pixelHeight: 180,
                windowWidth: 320 * 3,
                windowHeight: 180 * 3,
                orientation: OrientationType.Landscape,
                title: "morroEngine",
                enableVSync: true,
                startFullScreen: false,
                supportWideScreen: false
            );

            RandomManager.Initialize();
            SpriteManager.Initialize();
            SketchManager.Initialize();
            InputManager.Initialize();
            DebugManager.Initialize();
            CameraManager.Initialize();
            SoundManager.Initialize();
            SceneManager.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            spriteBatch.Dispose();

            AssetManager.UnloadContent();
            GraphicsManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime?.ElapsedGameTime.TotalSeconds;

            InputManager.Update();
            GraphicsManager.Update();
            WindowManager.Update();
            CameraManager.Update();
            SceneManager.Update();
            DebugManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            SceneManager.Draw(spriteBatch);
            SketchManager.Draw(spriteBatch);
            DebugManager.Draw(spriteBatch);
            WindowManager.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}


