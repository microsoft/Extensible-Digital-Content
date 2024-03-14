//--------------------------------------------------------------------------------------
// AudioView.xaml.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace EDCApp
{
    /// <summary>
    /// The AudioView class is responsible for how the AudioContent is displayed when navigated to. 
    /// </summary>
    public sealed partial class AudioView : Page
    {
        public AudioView()
        {
            this.InitializeComponent();

            // The source of the media player is the source of our global media player.
            // Eventually, this will be replaced with a custom player that is managed by the AudioService.
            // The developer will also want to create a viewmodel that will allow the UI to bind to the AudioService's properties.
            // For an example of this, look at the PopUpViewModel.
            AudioPlayer.SetMediaPlayer(AudioService.Instance.AudioPlayer);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            AudioContent content = (AudioContent)e.Parameter;
            AudioService.Instance.PlayAudio(content.ID);
            SharedUIViewModel.Instance.CurrentViewTitle = content.Name;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
    }
}
