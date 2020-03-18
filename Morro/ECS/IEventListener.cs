using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    interface IEventListener
    {
        void HandleEvent(object sender, EventArgs e);
    }
}
