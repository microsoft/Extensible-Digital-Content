//--------------------------------------------------------------------------------------
// InputService.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;
using System.Linq;
using Windows.Gaming.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace EDCApp
{
    /// <summary>
    /// This is a singleton service that listens for input from the keyboard and gamepad.
    /// </summary>
    internal class InputService
    {
        private static readonly Lazy<InputService> _instance = new Lazy<InputService>(() => new InputService());
        public static InputService Instance => _instance.Value;

        public delegate void InputUpdateHandler(object sender, InputEventArgs e);
        public event InputUpdateHandler InputUpdated;

        public Gamepad DefaultGamepad
        {
            get => _defaultGamepad;
            private set
            {
                _defaultGamepad = value;
            }
        }

        private readonly DispatcherTimer _inputPollingTimer;

        private CoreVirtualKeyStates[] _previousKeyStates;

        private GamepadButtons _previousGamepadButtons;

        private Gamepad _defaultGamepad;

        private InputService()
        {
            _inputPollingTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(200)
            };
            _inputPollingTimer.Tick += PollInput;
            _inputPollingTimer.Start();

            // 256 is the maximum number of virtual keys
            _previousKeyStates = new CoreVirtualKeyStates[256];

            Gamepad.GamepadAdded += GamepadGamepadAdded;
            Gamepad.GamepadRemoved += GamepadGamepadRemoved;

            // Set the initial default gamepad
            if (Gamepad.Gamepads.Count > 0)
            {
                DefaultGamepad = Gamepad.Gamepads.First();
            }
        }

        public void PollInput(object sender, object e)
        {
            var coreWindow = CoreWindow.GetForCurrentThread(); 
            var keyPressedAndReleased = new bool[256];

            // Add all keys that were pressed and released to the input event args
            foreach (VirtualKey key in Enum.GetValues(typeof(VirtualKey)))
            {
                var keyState = coreWindow.GetKeyState(key);
                int keyIndex = (int)key;

                if ((_previousKeyStates[keyIndex] & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down &&
                    (keyState & CoreVirtualKeyStates.Down) != CoreVirtualKeyStates.Down)
                {
                    // Mark the key as pressed and released
                    keyPressedAndReleased[keyIndex] = true;
                }
                else
                {
                    // Mark the key as not pressed
                    keyPressedAndReleased[keyIndex] = false;
                }
                _previousKeyStates[keyIndex] = keyState;
            }

            // Add all Gamepad buttons that were pressed and released to the input event args
            uint buttonsPressedAndReleased = 0;
            if (Gamepad.Gamepads.Count > 0) 
            {
                var gamePadReading = Gamepad.Gamepads[0].GetCurrentReading();
                uint currentGamepadButtons = (uint)gamePadReading.Buttons;

                // Get the buttons that were pressed and released with a bitwise operation
                buttonsPressedAndReleased = (uint)_previousGamepadButtons & ~currentGamepadButtons;

                _previousGamepadButtons = gamePadReading.Buttons;
            }

            var inputEventArgs = new InputEventArgs
            {
                KeyReadings = keyPressedAndReleased,
                ButtonReadings = (GamepadButtons)buttonsPressedAndReleased
            };

            // Raise the event, which ends up calling the HandleInput method in the MainWindowView
            InputUpdated?.Invoke(this, inputEventArgs);
        }

        protected virtual void OnInputUpdated(InputEventArgs e)
        {
            InputUpdated?.Invoke(this, e);
        }

        private void GamepadGamepadAdded(object sender, Gamepad e)
        {
            if (DefaultGamepad == null)
            {
                DefaultGamepad = e;
            }
        }

        private void GamepadGamepadRemoved(object sender, Gamepad e)
        {
            if (DefaultGamepad == e)
            {
                DefaultGamepad = Gamepad.Gamepads.FirstOrDefault();
            }
        }
        public void Cleanup()
        {
            Gamepad.GamepadAdded -= GamepadGamepadAdded;
            Gamepad.GamepadRemoved -= GamepadGamepadRemoved;
        }
        public void Resume()
        {
            Gamepad.GamepadAdded += GamepadGamepadAdded;
            Gamepad.GamepadRemoved += GamepadGamepadRemoved;
        }
    }

    // <summary> 
    // This class is used to pass input data to the event handler.
    // Only inputs that are pressed and released are passed.
    // Keyreadings: Index into the array with the appropriate typecasted virtualkey.
    // ButtonReadings is a bitfield. Bitwise & with the appropriate
    // GamepadButtons enum.
    // </summary>
    public class InputEventArgs : EventArgs
    {
        public bool[] KeyReadings { get; set; }
        public GamepadButtons ButtonReadings { get; set; }
    }
}
