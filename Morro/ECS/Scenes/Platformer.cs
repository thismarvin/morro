using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
using Morro.ECS.Entities;
using Morro.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS.Scenes
{
    class Platformer : Scene
    {
        public List<Polygon> BoundingBoxes { get; private set; }

        private BitmapFont notice;

        public Platformer() : base(SceneType.Platformer)
        {

        }

        protected override void Initialize()
        {
            BoundingBoxes = new List<Polygon>()
            {
                new AABB(0, WindowManager.PixelHeight - 32, WindowManager.PixelWidth, 32, Color.Black, VertexInformation.Static),
                new AABB(128 + 64, WindowManager.PixelHeight - 64 - 32, 64, 64, Color.Black, VertexInformation.Static),
                new RightTriangle(128, WindowManager.PixelHeight - 32 - 64, 64, 64, RightAnglePositionType.BottomRight, Color.Black, VertexInformation.Static),
            };

            notice = new BitmapFont(0, 32, "Collision is still a work in progress!", FontType.Probity);
            notice.SetLocation((SceneBounds.Width - notice.Width) / 2, notice.Y);
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

        public override void Update(GameTime gameTime)
        {
            UpdateEntities(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Sketch.CreateBackgroundLayer(spriteBatch, PICO8.SkyBlue);

            Sketch.AttachEffect(new DropShadow(Engine.RenderTarget, new Vector2(1, 1), WindowManager.Scale * 2));
            Sketch.Begin(spriteBatch);
            {
                foreach (Polygon polygon in BoundingBoxes)
                {
                    polygon.Draw(spriteBatch, CameraType.Dynamic);
                }

                notice.Draw(spriteBatch, CameraType.Dynamic);

                DrawEntities(spriteBatch);                
            }
            Sketch.End(spriteBatch);
        }
    }
}
