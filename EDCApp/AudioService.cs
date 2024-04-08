//--------------------------------------------------------------------------------------
// AudioService.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace EDCApp
{
    // This is a singleton service that manages the audio playback for the application.
    internal class AudioService
    {
        private static readonly Lazy<AudioService> _instance = new Lazy<AudioService>(() => new AudioService());
        public static AudioService Instance => _instance.Value;

        public delegate void SongChangedHandler(object sender, AudioContent e);
        public event SongChangedHandler SongChanged;

        public delegate void PlaybackStateChangedHandler(object sender, bool e);
        public event PlaybackStateChangedHandler PlayBackStateChanged;

        // In the future, this media player should be completely wrapped within this service.
        // This would allow the media player and related information such as active title, etc to be
        // maintained within this service. Windows MediaPlayer alone does not provide a way to
        // get this information.
        public MediaPlayer AudioPlayer { get; private set; }

        public int ActiveIndex
        {
            get { return _activeIndex; }
            private set
            {
                if (_activeIndex != value)
                {
                    _activeIndex = value;
                }
            }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            private set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                }
            }
        }
        
        private readonly object _playLock = new object();
        private MediaPlaybackState _stateAtInterrupt { get; set; }
        private List<AudioContent> _playList { get; set; }

        private AudioContent _activeAudioContent;

        private int _activeIndex;

        private bool _isPlaying;

        private AudioService()
        {
            AudioPlayer = new MediaPlayer();
            AudioPlayer.AutoPlay = true;
            AudioPlayer.MediaEnded += MediaEndedHandler;

            _playList = new List<AudioContent>();
            _activeIndex = 0;
            AudioPlayer.PlaybackSession.PlaybackStateChanged += MediaPlayBackStateChangedHandler;
        }

        protected virtual async void OnPlayBackStateChanged()
        {
            // Force the property change to run on the ui thread
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
            Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                PlayBackStateChanged?.Invoke(this, _isPlaying);
            });
        }

        protected virtual async void OnSongChanged(object sender, AudioContent e)
        {
            // Force the property change to run on the ui thread
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                           Windows.UI.Core.CoreDispatcherPriority.Normal,
                                      () =>
                                      {
                SongChanged?.Invoke(this, e);
            });
        }

        // Audio Player Methods
        public void InterruptAudio()
        {
            _stateAtInterrupt = AudioPlayer.PlaybackSession.PlaybackState;
            AudioPlayer.Pause();
        }

        public void ResumeAudio()
        {
            if (_stateAtInterrupt == MediaPlaybackState.Playing)
            {
                PlayAudio(_activeIndex);
            }
        }

        public void PlayAudio(int ID)
        {
            lock (_playLock)
            {
                if (_playList.Count > 0)
                {
                    // If the song is already playing, don't restart it. Also, if there is no song on fresh app start, play the first song in the list.
                    if(ID != _activeIndex || _activeAudioContent == null)
                    {
                        if (ID < 0)
                        {
                            _activeIndex = _playList.Count - 1;
                        }
                        else if (ID >= _playList.Count)
                        {
                            _activeIndex = 0;
                        }
                        else
                        {
                            _activeIndex = ID;
                        }

                        AudioPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///" + _playList[ActiveIndex].Path));
                        _activeAudioContent = _playList[ActiveIndex];
                        OnSongChanged(this, _activeAudioContent);
                    }
                    AudioPlayer.Play();
                }
            }
        }

        public void PlayAudio()
        {
            PlayAudio(_activeIndex);
        }
        
        public void PauseAudio()
        {
            if (_playList.Count > 0)
            {
                AudioPlayer.Pause();
            }
        }

        public void FastForward()
        {
            PlayAudio(_activeIndex + 1);
        }

        public void Rewind()
        {
            if (AudioPlayer.PlaybackSession.Position < TimeSpan.FromSeconds(3))
            {
                PlayAudio(_activeIndex - 1);
            }
            else
            {
                AudioPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(0);
            }
        }

        public string GetActiveSongTitle()
        {
            return _playList.Count == 0 ? "" : _playList[ActiveIndex].Name;
        }

        public void SetPlayList(List<AudioContent> playlist)
        {
            _playList = playlist;
        }

        public void AddSong(AudioContent song)
        {
            _playList.Add(song);
        }

        private void MediaEndedHandler(object sender, object e)
        {
            PlayAudio(_activeIndex + 1);
        }

        // Wrapper over windows MediaPlayer PlaybackSession.PlaybackStateChanged.
        // Event is pushed to the UI thread to allow for UI updates.
        private void MediaPlayBackStateChangedHandler(object sender, object e)
        {
            IsPlaying = AudioPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing;
            OnPlayBackStateChanged();
        }
        
        public void Cleanup()
        {
            AudioPlayer.MediaEnded -= MediaEndedHandler;
            AudioPlayer.PlaybackSession.PlaybackStateChanged -= MediaPlayBackStateChangedHandler;

            AudioPlayer.Pause();
        }

        public void Resume()
        {
            AudioPlayer.MediaEnded += MediaEndedHandler;
            AudioPlayer.PlaybackSession.PlaybackStateChanged += MediaPlayBackStateChangedHandler;

            // Resume audio playback if it was playing when the application was suspended.
            if (_isPlaying)
            {
                AudioPlayer.Play();
            }
        }
    }
}
