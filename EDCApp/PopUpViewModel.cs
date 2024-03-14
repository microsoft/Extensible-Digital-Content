//--------------------------------------------------------------------------------------
// PopUpViewModel.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;
using System.ComponentModel;

namespace EDCApp
{
    /// <summary>
    /// The PopUpViewModel class is responsible for managing the state of the media popup window
    /// throughout the application. It listens for changes in the audio service and
    /// updates the popup view accordingly.
    /// PopUpView.xaml.cs binds this PopUpViewModel into the PopUpView.xaml DataContext.
    /// </summary>
    public class PopUpViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // AudioService properties
        private AudioService _AudioService;

        public string CurrentSongTitle { get; private set; }
        public Boolean IsPlaying { get; private set; }

        // Viewmodel properties
        public string CurrentContentPath
        {
            get => _currentContentPath;
            set
            {
                _currentContentPath = value;
                OnPropertyChanged(nameof(CurrentContentPath));
            }
        }

        public string CurrentViewTitle
        {
            get => _currentViewTitle;
            set
            {
                _currentViewTitle = value;
                OnPropertyChanged(nameof(CurrentViewTitle));
            }
        }

        private string _currentContentPath;

        private string _currentViewTitle;

        /// <summary>
        /// The constructor for the PopUpViewModel class assigns the AudioService to a local variable.
        /// Event handlers are then assigned to the SongChanged and PlayBackStateChanged events.
        /// </summary>
        public PopUpViewModel()
        {
            _AudioService = AudioService.Instance;
            _AudioService.SongChanged += SongChange;
            _AudioService.PlayBackStateChanged += PlayBackStateChanged;
        }

        public void CleanUp()
        {
            _AudioService.SongChanged -= SongChange;
            _AudioService.PlayBackStateChanged -= PlayBackStateChanged;
        }

        public void Resume()
        {
            _AudioService.PlayBackStateChanged += PlayBackStateChanged;
            _AudioService.SongChanged += SongChange;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void SongChange(object sender, AudioContent e)
        {
            CurrentSongTitle = e.Name;
            OnPropertyChanged(nameof(CurrentSongTitle));
        }

        private void PlayBackStateChanged(object sender, bool e)
        {
            IsPlaying = e;
            OnPropertyChanged(nameof(IsPlaying));
        }
    }
}
