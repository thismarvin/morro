using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Morro.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Core
{
    public enum InputMode
    {
        Keyboard,
        Controller
    }

    static class InputManager
    {
        public static InputMode InputMode { get; private set; }
        public static InputHandler BasicInputHandler { get; private set; }

        private static Dictionary<string, InputProfile> profiles;

        public static void Initialize()
        {
            InputMode = InputMode.Keyboard;
            profiles = new Dictionary<string, InputProfile>();           

            LoadProfiles();

            BasicInputHandler = new InputHandler(PlayerIndex.One);
            BasicInputHandler.LoadProfile("Basic");            
        }

        public static void SetInputMode(InputMode inputMode)
        {
            InputMode = inputMode;
        }

        public static void RegisterProfile(InputProfile profile)
        {
            if (profiles.ContainsKey(profile.Name))
                throw new MorroException("An InputProfile with that name already exists; try a different name.", new ArgumentException("An item with the same key has already been added."));

            profiles.Add(profile.Name, profile);
        }

        public static InputProfile GetInputProfile(string name)
        {
            if (!profiles.ContainsKey(name.ToUpper(CultureInfo.InvariantCulture)))
                throw new MorroException("An InputProfile with that name does not exist.", new KeyNotFoundException());

            return profiles[name.ToUpper(CultureInfo.InvariantCulture)];
        }

        private static void LoadProfiles()
        {
            RegisterProfile(BasicInputProfile());
        }

        private static InputProfile BasicInputProfile()
        {
            InputProfile basic = new InputProfile("Basic");

            basic.CreateMapping(
                "Up",
                new Keys[] { Keys.W, Keys.Up },
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp, Buttons.RightThumbstickUp }
            );
            basic.CreateMapping(
                "Down",
                new Keys[] { Keys.S, Keys.Down },
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown, Buttons.RightThumbstickDown }
            );
            basic.CreateMapping(
                "Left",
                new Keys[] { Keys.A, Keys.Left },
                new Buttons[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft, Buttons.RightThumbstickLeft }
            );
            basic.CreateMapping(
                "Right",
                new Keys[] { Keys.D, Keys.Right },
                new Buttons[] { Buttons.DPadRight, Buttons.LeftThumbstickRight, Buttons.RightThumbstickRight }
            );
            basic.CreateMapping(
                "Start",
                new Keys[] { Keys.Enter },
                new Buttons[] { Buttons.Start }
            );

            return basic;
        }

        public static void Update()
        {
            Input.Keyboard.Update();
            Input.Mouse.Update();
            BasicInputHandler.Update();
        }
    }
}
