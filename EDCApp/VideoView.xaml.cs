//--------------------------------------------------------------------------------------
// VideoView.xaml.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EDCApp
{
    public sealed partial class VideoView : Page
    {
        public VideoView()
        {
            this.InitializeComponent();
            VideoPlayer.SetMediaPlayer(new Windows.Media.Playback.MediaPlayer());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AudioService.Instance.InterruptAudio();
            Content content = (Content)e.Parameter;
            VideoPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///" + content.Path));
            SharedUIViewModel.Instance.CurrentViewTitle = content.Name;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            AudioService.Instance.ResumeAudio();
            VideoPlayer.MediaPlayer.Pause();
        }
    }
}
