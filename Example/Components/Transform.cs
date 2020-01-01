using Morro.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Components
{
    struct Transform : IComponent
    {
        public float X { get;  set; }
        public float Y { get; private set; }
        public float Scale { get; private set; }
        public float Rotation { get; private set; }

        public Transform(float x, float y)
        {
            X = x;
            Y = y;
            Scale = 1;
            Rotation = 0;
        }

        public void SetLocation(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
