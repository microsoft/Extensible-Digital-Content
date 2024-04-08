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
    /// <summary>
    /// The FileNavViewModel class is responsible for managing the current directory that is displayed in the FileNavView.
    /// The FileNavView.xaml.cs binds this FileNavViewModel into the FileNavView.xaml DataContext.
    /// This allows the FileNavView to display dynamic content.
    /// </summary>
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
