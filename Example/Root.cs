using Example.Scenes;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example
{
    class Root : Engine
    {
        protected override void Initialize()
        {
            base.Initialize();

            WindowManager.SetTitle("Example");
            //WindowManager.EnableVSync(false);            

            ComponentManager.RegisterComponent("Transform");
            ComponentManager.RegisterComponent("PhysicsBody");
            ComponentManager.RegisterComponent("DrawableBody");

            SceneManager.RegisterScene(new Menu());
            SceneManager.RegisterScene(new Flocking());
            SceneManager.RegisterScene(new Platformer());
            SceneManager.RegisterScene(new Playground());

            SceneManager.QueueScene("Playground");
        }
    }
}
