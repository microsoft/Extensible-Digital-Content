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
    // This is a singleton service that listens for input from the keyboard and gamepad.
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

        // This will eventually be replaced with a proper keyboard input manager.
        // Right now, its just a placeholder bound to the shift key.
        private CoreVirtualKeyStates _previousShiftKeyState;

        // This will eventually be replaced with a proper controller input manager.
        // Right now, its just a placeholder bound to the up dpad on cotroller.
        private GamepadReading _previousGamepadReading;

        private Gamepad _defaultGamepad;

        private InputService()
        {
            _inputPollingTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(200)
            };
            _inputPollingTimer.Tick += PollInput;
            _inputPollingTimer.Start();

            Gamepad.GamepadAdded += GamepadGamepadAdded;
            Gamepad.GamepadRemoved += GamepadGamepadRemoved;

            // Set the initial default gamepad
            if (Gamepad.Gamepads.Count > 0)
            {
                DefaultGamepad = Gamepad.Gamepads.First();
            }
        }

        private void PollInput(object sender, object e)
        {
            var coreWindow = CoreWindow.GetForCurrentThread();
            var shiftKeyReading = coreWindow.GetKeyState(VirtualKey.Shift);

            InputEventArgs inputEventArgs = new InputEventArgs();

            if ((_previousShiftKeyState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down &&
                (shiftKeyReading & CoreVirtualKeyStates.Down) != CoreVirtualKeyStates.Down)
            {
                inputEventArgs.Key = VirtualKey.Shift;
            }
            _previousShiftKeyState = shiftKeyReading;

            if (DefaultGamepad != null)
            {
                var gamePadReading = DefaultGamepad.GetCurrentReading();

                if ((_previousGamepadReading.Buttons & GamepadButtons.DPadUp) == GamepadButtons.DPadUp &&
                    (gamePadReading.Buttons & GamepadButtons.DPadUp) != GamepadButtons.DPadUp)
                {
                    inputEventArgs.ButtonReading = GamepadButtons.DPadUp;
                }
                _previousGamepadReading = gamePadReading;
            }

            if(inputEventArgs.Key != 0 || inputEventArgs.ButtonReading != 0)
            {
                OnInputUpdated(inputEventArgs);
            }
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

    public class InputEventArgs : EventArgs
    {
        public GamepadButtons ButtonReading { get; set; }
        public Windows.System.VirtualKey Key { get; set; }
    }
}
