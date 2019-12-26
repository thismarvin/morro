using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    static class ComponentManager
    {
        private static Dictionary<string, int> components;
        private static int lastComponent;

        private static Dictionary<string, int> systems;
        private static int lastSystem;

        internal static void Initialize()
        {
            components = new Dictionary<string, int>
            {
                { FormatName("None"), 0 }
            };

            systems = new Dictionary<string, int>
            {
                { FormatName("None"), 0 }
            };
        }

        public static void RegisterComponent(string name)
        {
            string formattedName = FormatName(name);
            if (components.ContainsKey(name))
                throw new MorroException("A component with that name already exists; try a different name.", new ArgumentException("An item with the same key has already been added."));

            int id = 1 << lastComponent;
            components.Add(formattedName, id);
        }

        public static int GetComponentID(string name)
        {
            string formattedName = FormatName(name);
            if (!components.ContainsKey(formattedName))
                throw new Exception("A component with that name does not exist.", new KeyNotFoundException());

            return components[formattedName];
        }

        internal static string FormatName(string name)
        {
            return name.ToLowerInvariant();
        }

    }
}
