using Example.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using Morro.Input;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Scenes
{
    class Platformer : Scene
    {
        public List<Polygon> BoundingBoxes { get; private set; }

        private BitmapFont notice;

        public Platformer() : base("Platformer")
        {
            Initialize();
        }

        private void Initialize()
        {
            BoundingBoxes = new List<Polygon>()
            {
                new AABB(0, WindowManager.PixelHeight - 32, WindowManager.PixelWidth, 32, Color.Black, VertexInformation.Static),
                new AABB(128 + 64, WindowManager.PixelHeight - 64 - 32, 64, 64, Color.Black, VertexInformation.Static),
                new RightTriangle(128, WindowManager.PixelHeight - 32 - 64, 64, 64, RightAnglePositionType.BottomRight, Color.Black, VertexInformation.Static),
            };

            notice = new BitmapFont(0, 32, "Collision is still a work in progress!", FontType.Probity);
            notice.SetLocation((SceneBounds.Width - notice.Width) / 2, notice.Y);

            InputProfile inputProfile = new InputProfile("Player");
            inputProfile.CreateMapping(
                "Left",
                new Keys[] { Keys.A, Keys.Left },
                new Buttons[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft, Buttons.RightThumbstickLeft }
            );
            inputProfile.CreateMapping(
                "Right",
                new Keys[] { Keys.D, Keys.Right },
                new Buttons[] { Buttons.DPadRight, Buttons.LeftThumbstickRight, Buttons.RightThumbstickRight }
            );
            inputProfile.CreateMapping(
                "Jump",
                new Keys[] { Keys.W, Keys.Up, Keys.Space },
                new Buttons[] { Buttons.A }
            );
            InputManager.RegisterProfile(inputProfile);

            AssetManager.LoadImage("Sprites", "Assets/Sprites/Sprites");
            SpriteManager.AddSpriteData("Player", 0, 0, 16, 32, "Sprites");
        }

        public override void LoadScene()
        {
            Entities.Clear();
            Entities.Add(new Player(16, 16, PlayerIndex.One));
        }

        public override void UnloadScene()
        {
            Entities.Clear();
        }

        public override void Update()
        {
            UpdateEntities();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sketch.CreateBackgroundLayer(spriteBatch, PICO8.SkyBlue);

            Sketch.AttachEffect(new DropShadow(Engine.RenderTarget, new Vector2(1, 1), WindowManager.Scale * 2));
            Sketch.Begin(spriteBatch);
            {
                foreach (Polygon polygon in BoundingBoxes)
                {
                    polygon.Draw(spriteBatch, Camera);
                }

                notice.Draw(spriteBatch, Camera);

                DrawEntities(spriteBatch);                
            }
            Sketch.End(spriteBatch);
        }
    }
}
