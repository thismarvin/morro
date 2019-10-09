using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
using Morro.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS.Scenes
{
    class Menu : Scene
    {
        private List<Polygon> polygons;
        private BitmapFont title;
        private Color[] colors;
        private float titleAngle;

        public Menu() : base(SceneType.Menu)
        {

        }

        protected override void Initialize()
        {
            polygons = new List<Polygon>();
            colors = new Color[] { PICO8.BloodRed, PICO8.ConeOrange, PICO8.DarkPurple, PICO8.FleshPink, PICO8.GrassGreen, PICO8.LightGray, PICO8.LightPurple, PICO8.OceanBlue, PICO8.PeachWhite, PICO8.SkyBlue, PICO8.TaxiYellow };

            title = new BitmapFont(0, 0, "morro", FontType.Sparge);
            title.SetLocation((WindowManager.PixelWidth - title.Width) / 2f, (WindowManager.PixelHeight - title.Height) / 2f);

            titleAngle = MathHelper.PiOver2;
        }

        public override void LoadScene()
        {
            titleAngle = MathHelper.PiOver2;

            polygons.Clear();

            int size;
            for (int i = 0; i < 47; i++)
            {
                size = RandomHelper.Range(64, 256);
                polygons.Add(new RegularPolygon(RandomHelper.Range(-128, WindowManager.PixelWidth), RandomHelper.Range(-128, WindowManager.PixelHeight), size, size, RandomHelper.Range(3, 8), 0, colors[RandomHelper.Range(0, colors.Length - 1)], VertexInformation.Static));
                polygons[polygons.Count - 1].SetRotationOffset(polygons[polygons.Count - 1].Width / 2, polygons[polygons.Count - 1].Height / 2);
            }
            for (int i = 0; i < 3; i++)
            {
                size = RandomHelper.Range(64, 256);
                polygons.Add(new Star(RandomHelper.Range(-128, WindowManager.PixelWidth), RandomHelper.Range(-128, WindowManager.PixelHeight), size, size, 0, colors[RandomHelper.Range(0, colors.Length - 1)], VertexInformation.Static));
                polygons[polygons.Count - 1].SetRotationOffset(polygons[polygons.Count - 1].Width / 2, polygons[polygons.Count - 1].Height / 2);
            }
        }

        public override void UnloadScene()
        {

        }

        private void AnimateTitle()
        {
            titleAngle += Engine.DeltaTime * 5;

            float startingScale = 1;
            float maxScale = 3;
            float multiplier = (maxScale - startingScale) / 2;

            title.SetScale(multiplier * 2 + (float)Math.Cos(titleAngle) * multiplier, multiplier * 2 + (float)Math.Cos(titleAngle) * multiplier);
            title.SetLocation((WindowManager.PixelWidth - title.Width) / 2f, (WindowManager.PixelHeight - title.Height) / 2f);
            title.SetRotation((float)Math.Cos(titleAngle / 2) / 2);
        }

        private void RotatePolygons()
        {
            foreach (Polygon p in polygons)
            {
                p.Rotate(p.Width / 100f * Engine.DeltaTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            UpdateEntities(gameTime);            
            RotatePolygons();
            AnimateTitle();
            
            if (Input.Keyboard.Pressed(Keys.Enter))
            {
                SceneManager.QueueScene(SceneType.Platformer);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sketch.CreateBackgroundLayer(spriteBatch, PICO8.MidnightBlack);

            Sketch.AttachEffect(new ChromaticAberration(Engine.RenderTarget, 10 * WindowManager.Scale));
            Sketch.DisableRelay();
            Sketch.Begin(spriteBatch);                      
            {                
                foreach (Polygon p in polygons)
                {
                    p.Draw(spriteBatch, CameraType.Static);
                }
            }
            Sketch.End(spriteBatch);
            SketchHelper.ApplyGaussianBlur(spriteBatch, Sketch.InterceptRelay(), 2);

            Vector2 direction = VectorHelper.FromAngle(title.Rotation + MathHelper.PiOver4);
            Sketch.AttachEffect(new DropShadow(Engine.RenderTarget, direction, WindowManager.Scale * title.Scale.X * 1.25f, Color.Black));
            Sketch.Begin(spriteBatch);
            {
                title.Draw(spriteBatch, CameraType.Static);
            }
            Sketch.End(spriteBatch);
        }
    }
}
