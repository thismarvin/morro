using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    internal interface IMorroSystem
    {
        bool Enabled { get; set; }
    }
}
