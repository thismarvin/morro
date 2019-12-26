using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    abstract class Component
    {
        public string Name { get; private set; }
        public int ID { get; private set; }

        public Component(string name)
        {
            Name = ComponentManager.FormatName(name);
            ID = ComponentManager.GetComponentID(Name);
        }
    }
}
