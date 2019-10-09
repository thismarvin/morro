using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.Graphics;
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
            AssetManager.LoadContent(Content);            
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
            SketchManager.Initialize();
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

        private void UpdateInput()
        {
            Input.Keyboard.Update();
            Input.Mouse.Update();

#if !__IOS__ && !__TVOS__
            if (Input.Keyboard.Pressed(Keys.Escape))
            {
                Exit();
            }
#endif
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateInput();

            GraphicsManager.Update();
            WindowManager.Update(gameTime);
            CameraManager.Update();
            SceneManager.Update(gameTime);
            DebugManager.Update();
           
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            SceneManager.Draw(spriteBatch);
            //SketchManager.AttachEffect(new Dither(RenderTarget));
            //SketchManager.AttachEffect(new Quantize(4));
            //SketchManager.AttachEffect(new Palette());
            SketchManager.Draw(spriteBatch);

            DebugManager.Draw(spriteBatch);
            WindowManager.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}


