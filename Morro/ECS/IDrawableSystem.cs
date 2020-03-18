using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    interface IDrawableSystem : IMorroSystem, IComparable<IDrawableSystem>
    {
        int Priority { get; set; }

        void Draw(Camera camera);
    }
}
