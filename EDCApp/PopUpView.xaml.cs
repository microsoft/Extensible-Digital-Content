//--------------------------------------------------------------------------------------
// PopUpView.xaml.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Core;

namespace EDCApp
{
    public sealed partial class PopUpView : UserControl
    {
        private PopUpViewModel _viewModel;
        public PopUpView()
        {
            this.InitializeComponent();
            _viewModel = new PopUpViewModel();
            Window.Current.SizeChanged += WindowSizeChanged;
            this.Loaded += PopupViewLoaded;
            this.Unloaded += OnPopupClosed;
            this.DataContext = _viewModel;
        }

        public void CenterPopup()
        {
            if (this.Parent is Popup popup)
            {
                var windowBounds = Window.Current.Bounds;
                var popupWidth = this.ActualWidth;
                var popupHeight = this.ActualHeight;

                popup.HorizontalOffset = (windowBounds.Width - popupWidth) / 2;
                popup.VerticalOffset = (windowBounds.Height - popupHeight) / 2;
            }
        }
        public void CleanUp()
        {
            _viewModel.CleanUp();
            Window.Current.SizeChanged -= WindowSizeChanged;
            this.Loaded -= PopupViewLoaded;
            this.Unloaded -= OnPopupClosed;
        }

        public void Resume()
        {
            Window.Current.SizeChanged += WindowSizeChanged;
            this.Loaded += PopupViewLoaded;
            this.Unloaded += OnPopupClosed;
            _viewModel.Resume();
        }

        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            CenterPopup();
        }

        private void OnPopupClosed(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= WindowSizeChanged;
        }

        private void PopupViewLoaded(object sender, RoutedEventArgs e)
        {
            CenterPopup();
        }

        private void ClickRewind(object sender, RoutedEventArgs e)
        {
            AudioService.Instance.Rewind();
        }

        private void ClickPlay(object sender, RoutedEventArgs e)
        {
            if (!AudioService.Instance.GetActiveSongTitle().Equals(""))
            {
                if (AudioService.Instance.IsPlaying)
                {
                    PlayPauseIcon.Symbol = Symbol.Play;
                    AudioService.Instance.PauseAudio();
                }
                else
                {
                    PlayPauseIcon.Symbol = Symbol.Pause;
                    AudioService.Instance.PlayAudio();
                }
            }
        }

        private void ClickFastForward(object sender, RoutedEventArgs e)
        {
            AudioService.Instance.FastForward();
        }
    }
}
