//--------------------------------------------------------------------------------------
// SettingsView.xaml.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EDCApp
{
    /// <summary>
    /// The settings page for the application.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            this.InitializeComponent();
            this.ContentFrame.Navigate(typeof(ControlsSettingView));
        }

        private void NavigationMenuSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContentFrame == null)
            {
                // The frame is not yet initialized, so do nothing.
                return;
            }

            if (e.AddedItems[0] is ListViewItem item)
            {
                switch (item.Tag.ToString())
                {
                    case "Controls":
                        this.ContentFrame.Navigate(typeof(ControlsSettingView));
                        break;
                    case "Quit":
                        Application.Current.Exit();
                        break;
                }
            }
        }
    }
}
