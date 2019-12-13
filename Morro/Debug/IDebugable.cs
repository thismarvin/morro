using Microsoft.Xna.Framework.Graphics;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Debug
{
    interface IDebugable
    {
        void Debug(SpriteBatch spriteBatch, Camera camera);
    }
}
