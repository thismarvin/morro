using Microsoft.Xna.Framework;
using Morro.Input;
using System;
using System.Collections.Generic;
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
        public static Dictionary<string, InputProfile> Profiles { get; private set; }
        public static Dictionary<PlayerIndex, InputHandler> Handlers { get; private set; }
        public static InputMode InputMode { get; private set; }

        public static void Initialize()
        {
            Profiles = new Dictionary<string, InputProfile>();
            Handlers = new Dictionary<PlayerIndex, InputHandler>();
            InputMode = InputMode.Keyboard;

            LoadProfiles();
        }

        public static void SetInputMode(InputMode inputMode)
        {
            InputMode = inputMode;
        }        

        public static void RegisterProfile(InputProfile profile)
        {
            if (Profiles.ContainsKey(profile.Name))
                throw new Exception("An InputProfile with that name already exists; try a different name.");

            Profiles.Add(profile.Name, profile);
        }

        //public static InputHandler CreateInputHandler(PlayerIndex playerIndex)
        //{
        //    if (Handlers.ContainsKey(playerIndex))
        //        throw new Exception("An InputHandler with that PlayerIndex already exists; try a different PlayerIndex.");
        //    InputHandler inputHandler = new InputHandler()
        //}

        public static void Update()
        {

        }

        private static void LoadProfiles()
        {

        }
    }
}
