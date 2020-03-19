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
        public static RenderTarget2D RenderTarget { get => WindowManager.RenderTarget; }
        public static float DeltaTime { get; private set; }
        public static TimeSpan TotalGameTime { get; private set; }

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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            AssetManager.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            //spriteBatch.Dispose();

            AssetManager.UnloadContent();
            GeometryManager.UnloadContent();
            GraphicsManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime?.ElapsedGameTime.TotalSeconds;
            TotalGameTime = gameTime.TotalGameTime;

            InputManager.Update();
            WindowManager.Update();
            CameraManager.Update();
            SceneManager.Update();
            DebugManager.Update();
            SoundManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            SceneManager.Draw();
            SketchManager.Draw();
            DebugManager.Draw();
            WindowManager.Draw();

            base.Draw(gameTime);
        }
    }
}


