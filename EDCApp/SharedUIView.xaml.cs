//--------------------------------------------------------------------------------------
// SharedUIView.xaml.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EDCApp
{
    /// <summary>
    /// The SharedUIView class controls how the SharedUI is displayed.
    /// The SharedUIView contains a nav bar and a frame to display the active content.
    /// </summary>
    public sealed partial class SharedUIView : Page
    {
        public SharedUIView()
        {
            this.InitializeComponent();
            this.DataContext = SharedUIViewModel.Instance;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ContentFrame.BackStack.Clear();
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Navigate to the root directory of our content
            var rootDir = new FolderContent("DigitalContent", "DigitalContent");
            ContentFrame.Navigate(typeof(FileNavView), rootDir);
            base.OnNavigatedTo(e);
        }

        private void NavBackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
            {
                // If there are multiple nav menu items, this can be removed.
                NavView.SelectedItem = null;

                ContentFrame.GoBack();
            }
            else
            {
                var rootFrame = this.Frame;
                rootFrame.Navigate(typeof(TitleScreenView));
            }
        }
        private void NavViewItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                if (ContentFrame.CurrentSourcePageType != typeof(SettingsView))
                    ContentFrame.Navigate(typeof(SettingsView));
                else
                {
                    // If there are multiple nav menu items, this can be removed.
                    NavView.SelectedItem = null;

                    ContentFrame.GoBack();
                }
            }
        }
    }
}
