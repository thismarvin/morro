using Example.Components;
using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Entities
{
    static class Yoman
    {
        public static int Create(Scene scene, int x, int y)
        {
            int entity = scene.AllocateEntity();

            scene.Require("Transform");
            scene.Require("PhysicsBody");
            scene.Require("DrawableBody");

            scene.AssignComponent(entity, new Transform(x, y));
            scene.AssignComponent(entity, new PhysicsBody(10));
            scene.AssignComponent(entity, new DrawableBody());

            scene.RegisterEntity(entity);

            return entity;
        }
    }
}
