using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Example.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.ECS;

namespace Example.Systems
{
    class PhysicsSystem : MorroSystem
    {
        private IComponent[] transforms;
        private IComponent[] physicsBodies;

        public PhysicsSystem() : base(4)
        {
            Require(typeof(Transform), typeof(PhysicsBody));
        }

        public override void GrabData(Scene scene)
        {
            transforms = scene.GetData<Transform>();
            physicsBodies = scene.GetData<PhysicsBody>();
        }

        public override void UpdateEntity(int entity)
        {
            Transform transform = (Transform)transforms[entity];
            PhysicsBody physicsBody = (PhysicsBody)physicsBodies[entity];

            transform.SetLocation(transform.X + physicsBody.Speed * Engine.DeltaTime, transform.Y);

            transforms[entity] = transform;
        }

        public override void DrawEntity(int entity, SpriteBatch spriteBatch)
        {

        }
    }
}
