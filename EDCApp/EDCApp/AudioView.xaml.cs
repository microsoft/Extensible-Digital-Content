using DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Windows.Media.Playback;
using Windows.Media.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace EDCApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AudioView : Page
    {
        private bool isLoaded;
        public AudioView()
            //no binding because we need a global media player. You cant bind a media player. Regarding the source, we are modifying the source of our global media player. not the media player attached to the
            //media player element.
        {
            this.InitializeComponent();

            AudioViewModel model = AudioViewModel.Instance;
            isLoaded = false;
            AudioPlayer.SetMediaPlayer(AudioViewModel.Instance.AudioPlayer);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            IClickableContent content = (IClickableContent)e.Parameter;
            AudioViewModel.Instance.AudioPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///" + content.Path));
            SharedUIViewModel.Instance.CurrentViewTitle = content.Name;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            AudioPlayer.MediaPlayer.Pause();
        }
    }
}
