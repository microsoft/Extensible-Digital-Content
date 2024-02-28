//--------------------------------------------------------------------------------------
// FileNavViewModel.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EDCApp
{
    public class FileNavViewModel : INotifyPropertyChanged
    {
        public FileNavViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private ObservableCollection<Content> _clickableContent;
        public ObservableCollection<Content> ClickableContents
        {
            get => _clickableContent;
            set
            {
                _clickableContent = value;
                OnPropertyChanged(nameof(ClickableContents));
            }
        }

        public void PopulateContent(string relativePath)
        {
            ClickableContents = DataModel.ContentDictionary[relativePath];
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
