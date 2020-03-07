using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    interface IDrawableSystem
    {
        void Draw(Camera camera);
        void DrawEntity(int entity, Camera camera);
    }
}
