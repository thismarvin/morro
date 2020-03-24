using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    /// <summary>
    /// Provides functionality for a <see cref="MorroSystem"/> to update entities.
    /// </summary>
    interface IUpdateableSystem : IMorroSystem
    {
        void Update();
    }
}
