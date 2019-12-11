using Microsoft.Xna.Framework.Input;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Input
{
    class InputMapping
    {
        public string Name { get; private set; }
        public Keys[] Keys { get; private set; }
        public Buttons[] Buttons { get; private set; }

        public InputMapping(string name, Keys[] keys) : this(name, keys, new Buttons[0])
        {

        }

        public InputMapping(string name, Keys[] keys, Buttons[] buttons)
        {
            Name = InputManager.FormatName(name);
            Keys = keys;
            Buttons = buttons;
        }

        public void Remap(Keys[] keys, Buttons[] buttons)
        {
            Keys = keys;
            Buttons = buttons;
        }
    }
}
