﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Input
{
    /// <summary>
    /// An extension of the <see cref="GamePad"/> class that provides additional functionality to interface with a given gamepad.
    /// </summary>
    class MoreGamePad
    {
        public PlayerIndex PlayerIndex { get; private set; }
        public bool Vibrating { get; private set; }
        public bool IsConnected { get => currentGamePadState.IsConnected; }

        private bool isBeingUpdated;
        private GamePadState previousGamePadState;
        private GamePadState currentGamePadState;

        private readonly Timer vibrationTimer;

        /// <summary>
        /// Creates an instance of a <see cref="MoreGamePad"/> that is attached to a given <see cref="Microsoft.Xna.Framework.PlayerIndex"/>.
        /// </summary>
        /// <param name="playerIndex">The player index this gamepad will attach to.</param>
        public MoreGamePad(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            vibrationTimer = new Timer(0);
        }

        /// <summary>
        /// Returns whether or not the give button was just pressed.
        /// </summary>
        /// <param name="button">The gamepad button that should be tested.</param>
        /// <returns>Whether or not the give button was just pressed.</returns>
        public bool Pressed(Buttons button)
        {
            VerifyUpdateIsCalled();

            if (!previousGamePadState.IsButtonDown(button) && currentGamePadState.IsButtonDown(button))
            {
                InputManager.InputMode = InputMode.Controller;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns whether or not the given button is currently being held down.
        /// </summary>
        /// <param name="button">The gamepad button that should be tested.</param>
        /// <returns>Whether or not the given button is currently being held down.
        public bool Pressing(Buttons button)
        {
            VerifyUpdateIsCalled();

            if (currentGamePadState.IsButtonDown(button))
            {
                InputManager.InputMode = InputMode.Controller;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set the vibration of the gamepad. (Note that once you set the vibration the gamepad will not stop vibrating until its vibration is reset to zero).
        /// </summary>
        /// <param name="leftMotorIntensity">The speed of the left motor, between 0.0 and 1.0.</param>
        /// <param name="rightMotorIntensity">The speed of the right motor, between 0.0 and 1.0.</param>
        public void SetVibration(double leftMotorIntensity, double rightMotorIntensity)
        {
            if (InputManager.InputMode != InputMode.Controller)
                return;

            GamePad.SetVibration(PlayerIndex, (float)leftMotorIntensity, (float)rightMotorIntensity);
        }

        /// <summary>
        /// Vibrates the gamepad at a given intensity and for a given duration.
        /// </summary>
        /// <param name="leftMotorIntensity">The speed of the left motor, between 0.0 and 1.0.</param>
        /// <param name="rightMotorIntensity">The speed of the right motor, between 0.0 and 1.0.</param>
        /// <param name="duration">The duration of the vibration in milliseconds.</param>
        public void Vibrate(double leftMotorIntensity, double rightMotorIntensity, float duration)
        {
            if (Vibrating || InputManager.InputMode != InputMode.Controller)
                return;

            Vibrating = true;
            vibrationTimer.Duration = duration;
            vibrationTimer.Reset();
            vibrationTimer.Start();

            SetVibration(leftMotorIntensity, rightMotorIntensity);
        }

        /// <summary>
        /// Resets the vibration of the gamepad to zero.
        /// </summary>
        public void ResetVibration()
        {
            Vibrating = false;
            vibrationTimer.Stop();

            SetVibration(0, 0);
        }

        public void Update()
        {
            isBeingUpdated = true;
            previousGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex);

            UpdateVibration();
        }

        private void UpdateVibration()
        {
            if (!Vibrating)
                return;

            vibrationTimer.Update();

            if (vibrationTimer.Done || InputManager.InputMode == InputMode.Keyboard)
            {
                ResetVibration();
            }
        }

        private void VerifyUpdateIsCalled()
        {
            if (!isBeingUpdated)
                throw new MorroException("Make sure to call your MoreGamePad's Update() method before you use any of the built in methods.", new MethodExpectedException());
        }
    }
}
