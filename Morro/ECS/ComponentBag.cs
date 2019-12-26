using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class ComponentBag
    {
        public int Components { get; private set; }

        public bool AddComponent(string componentName)
        {
            int component = ComponentManager.GetComponentID(componentName);

            if ((Components & component) == 0)
            {
                Components |= component;
                return true;
            }
            return false;
        }

        public bool AddComponent(params string[] componentNames)
        {
            bool result = false;
            for (int i = 0; i < componentNames.Length; i++)
            {
                result |= AddComponent(componentNames[i]);
            }
            return result;
        }

        public void AddRange(string[] componentNames)
        {
            for (int i = 0; i < componentNames.Length; i++)
            {
                Components |= ComponentManager.GetComponentID(componentNames[i]);
            }
        }

        public bool Remove(string componentName)
        {
            if (!Contains(componentName))
            {
                return false;
            }

            Components &= ~ComponentManager.GetComponentID(componentName);

            return true;
        }

        public bool Contains(string componentName)
        {
            return ((Components & ComponentManager.GetComponentID(componentName)) != 0);
        }

        public bool Contains(ComponentBag componentBag)
        {
            return ((Components & componentBag.Components) != 0);
        }
    }
}
