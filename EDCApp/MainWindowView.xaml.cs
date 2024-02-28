﻿//--------------------------------------------------------------------------------------
// MainWindowView.xaml.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using Windows.Gaming.Input;
using Windows.UI.Xaml.Controls;

namespace EDCApp
{
    public sealed partial class MainWindowView : Page
    {
        private PopUpView _popUpView;
        public MainWindowView()
        {
            this.InitializeComponent();
            _popUpView = new PopUpView();
            MediaPopup.Child = _popUpView;
            ApplicationFrame.Navigate(typeof(TitleScreenView));
            InputService.Instance.InputUpdated += HandleInput;
        }

        private void HandleInput(object sender, InputEventArgs e)
        {
            if ((e.ButtonReading & GamepadButtons.DPadUp) == GamepadButtons.DPadUp || e.Key == Windows.System.VirtualKey.Shift)
            {
                MediaPopup.IsOpen = !MediaPopup.IsOpen;
            }
        }

        public void CleanUp()
        {
            _popUpView.CleanUp();
            InputService.Instance.InputUpdated -= HandleInput;
        }
        public void Resume()
        {
            InputService.Instance.InputUpdated += HandleInput;
            _popUpView.Resume();
        }

    }
}