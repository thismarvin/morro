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
            WindowManager.EnableVSync(false);         

            //SceneManager.RegisterScene(new Playground());
            SceneManager.RegisterScene(new Flocking());

            SceneManager.QueueScene("Flocking");
        }
    }
}
