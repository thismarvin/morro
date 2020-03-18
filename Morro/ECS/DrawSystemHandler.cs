﻿using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class DrawSystemHandler
    {
        private readonly MorroSystem parent;
        private readonly Action<int, Camera> onDraw;

        public DrawSystemHandler(MorroSystem parent, Action<int, Camera> onDraw)
        {
            this.parent = parent;
            this.onDraw = onDraw;
        }

        public void Draw(Camera camera)
        {
            foreach (int entity in parent.Entities)
            {
                onDraw(entity, camera);
            }
        }
    }
}
