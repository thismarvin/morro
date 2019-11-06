using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Input
{
    class InputProfile
    {
        public string Name { get; private set; }
        public Dictionary<string, InputMapping> InputMappings { get; private set; }

        public InputProfile(string name)
        {
            Name = name;            
            InputMappings = new Dictionary<string, InputMapping>();
        }

        public void CreateMapping(string name, Keys[] keys)
        {
            InputMappings.Add(name.ToUpper(), new InputMapping(name, keys));
        }

        public void CreateMapping(string name, Keys[] keys, Buttons[] buttons)
        {
            InputMappings.Add(name.ToUpper(), new InputMapping(name, keys, buttons));
        }
    }
}
