//--------------------------------------------------------------------------------------
// SharedUIViewModel.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;
using System.ComponentModel;

namespace EDCApp
{
    // All Content Views exist within the same instance of this SharedUIViewModel
    public class SharedUIViewModel : INotifyPropertyChanged
    {
        private static readonly Lazy<SharedUIViewModel> _instance = new Lazy<SharedUIViewModel>(() => new SharedUIViewModel());
        public static SharedUIViewModel Instance => _instance.Value;

        public event PropertyChangedEventHandler PropertyChanged;

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

        private static string _currentViewTitle;

        private static string _currentContentPath;

        private SharedUIViewModel()
        {
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
