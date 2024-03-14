//--------------------------------------------------------------------------------------
// SharedUIView.xaml.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
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
                ContentFrame.GoBack();
            }
            else
            {
                var rootFrame = this.Frame;
                rootFrame.Navigate(typeof(TitleScreenView));
            }
        }
    }
}
