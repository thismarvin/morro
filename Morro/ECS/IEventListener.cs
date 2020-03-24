﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    /// <summary>
    /// Provides functionality to listen to and handle a given event.
    /// </summary>
    interface IEventListener
    {
        void HandleEvent(object sender, EventArgs e);
    }
}
