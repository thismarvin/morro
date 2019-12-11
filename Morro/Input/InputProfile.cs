using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Input
{
    class InputProfile
    {
        public string Name { get; private set; }

        private readonly Dictionary<string, InputMapping> inputMappings;

        public InputProfile(string name)
        {
            Name = InputManager.FormatName(name);
            inputMappings = new Dictionary<string, InputMapping>();
        }

        public void CreateMapping(string name, Keys[] keys)
        {
            CreateMapping(name, keys, new Buttons[0]);
        }

        public void CreateMapping(string name, Keys[] keys, Buttons[] buttons)
        {
            string formattedName = InputManager.FormatName(name);
            if (inputMappings.ContainsKey(formattedName))
                throw new MorroException("An InputMapping with that name already exists; try a different name.\nIf you are trying to remap a pre-exsiting InputMapping then use the Remap() method.", new ArgumentException("An item with the same key has already been added."));

            InputMapping inputMapping = new InputMapping(formattedName, keys, buttons);
            inputMappings.Add(inputMapping.Name, inputMapping);
        }

        public InputMapping GetInputMapping(string name)
        {
            string formattedName = InputManager.FormatName(name);
            if (!inputMappings.ContainsKey(formattedName))
                throw new MorroException("An InputMapping with that name does not exist.", new KeyNotFoundException());

            return inputMappings[formattedName];
        }

        public void Remap(string name, Keys[] keys)
        {
            Remap(name, keys, new Buttons[0]);
        }

        public void Remap(string name, Keys[] keys, Buttons[] buttons)
        {
            string formattedName = InputManager.FormatName(name);
            if (!inputMappings.ContainsKey(formattedName))
                throw new Exception("An InputMapping with that name does not exist. If you are trying to create a new InputMapping then use the CreateMapping() method.", new KeyNotFoundException());

            GetInputMapping(formattedName).Remap(keys, buttons);
        }
    }
}
