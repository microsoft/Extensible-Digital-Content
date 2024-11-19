//--------------------------------------------------------------------------------------
// TitleScreenView.xaml.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EDCApp
{
    /// <summary>
    /// The TitleScreenView class controls the title screen of the application.
    /// </summary>
    public sealed partial class TitleScreenView : Page
    {
        public TitleScreenView()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = this.Frame;
            rootFrame.Navigate(typeof(SharedUIView), "DigitalContent");
        }
    }
}
