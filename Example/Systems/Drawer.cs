using System;
using System.Collections.Generic;
using System.Text;
using Example.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.ECS;
using Morro.Graphics;

namespace Example.Systems
{
    class Drawer : Morro.ECS.System
    {
        public Drawer() : base("Drawer")
        {
            Require
            (
                "Transform",
                "DrawableBody"
            );
        }

        public override void Update(Scene scene)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, Scene scene)
        {
            Component[] transforms = scene.GetData("Transform");
            Component[] drawable = scene.GetData("DrawableBody");

            Transform transform;
            DrawableBody drawableBody;
            for (int i = 0; i < registeredEntities.Count; i++)
            {               
                transform = (Transform)transforms[registeredEntities[i]];
                drawableBody = (DrawableBody)drawable[registeredEntities[i]];

                drawableBody.SetPosition(transform.X, transform.Y);
                drawableBody.Body.Draw(spriteBatch, Morro.Core.CameraType.Static);
            }
        }
    }
}
