using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    interface IUpdateableSystem : IMorroSystem
    {
        void Update();
    }
}
