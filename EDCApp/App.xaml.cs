//--------------------------------------------------------------------------------------
// App.xaml.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EDCApp
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += OnResuming;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                Window.Current.Content = rootFrame;
            }

            if(rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainWindowView));
            }

            Window.Current.Activate();

            DataModel.InitializeContent();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // Clean up UWP
            if (Window.Current.Content is Frame rootFrame && rootFrame.Content is MainWindowView currentPage)
            {
                currentPage.CleanUp();
            }

            // CleanUp Services last
            InputService.Instance.Cleanup();
            AudioService.Instance.Cleanup();
            deferral.Complete();
        }

        private void OnResuming(object sender, Object e)
        {
            // CleanUp Services first
            InputService.Instance.Resume();
            AudioService.Instance.Resume();

            // Resume UWP
            if (Window.Current.Content is Frame rootFrame && rootFrame.Content is MainWindowView currentPage)
            {
                currentPage.Resume();
            }
        }
    }
}
