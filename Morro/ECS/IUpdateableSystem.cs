using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    interface IUpdateableSystem
    {
        void Update();
        void UpdateEntity(int entity);        
    }
}
