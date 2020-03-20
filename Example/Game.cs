using Example.Scenes;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example
{
    class Game : Engine
    {
        protected override void Initialize()
        {
            base.Initialize();

            WindowManager.SetTitle("Example");

            SceneManager.RegisterScene(new Fireworks());
            SceneManager.RegisterScene(new Flocking());

            SceneManager.QueueScene("Flocking");
        }
    }
}
