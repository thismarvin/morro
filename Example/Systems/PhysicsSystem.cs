using System;
using System.Collections.Generic;
using System.Text;
using Example.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;

namespace Example.Systems
{
    class PhysicsSystem : Morro.ECS.System
    {
        public PhysicsSystem() : base("PhysicsSystem")
        {
            Require("Transform", "PhysicsBody");
        }

        public override void Update(Scene scene)
        {
            Component[] transforms = scene.GetData("Transform");
            Component[] physicBodies = scene.GetData("PhysicsBody");

            Transform transform;
            PhysicsBody physicsBody;
            for (int i = 0; i < registeredEntities.Count; i++)
            {
                transform = (Transform)transforms[registeredEntities[i]];
                physicsBody = (PhysicsBody)physicBodies[registeredEntities[i]];

                transform.SetLocation(transform.X + physicsBody.Speed * Engine.DeltaTime, transform.Y);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Scene scene)
        {

        }
    }
}
