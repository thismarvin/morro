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

            SceneManager.RegisterScene(new Menu());
            SceneManager.RegisterScene(new Flocking());
            SceneManager.RegisterScene(new Platformer());
            SceneManager.QueueScene("Menu");
        }
    }
}
