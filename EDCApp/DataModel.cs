//--------------------------------------------------------------------------------------
// DataModel.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;

namespace EDCApp
{
    public static class DataModel
    {
        public static Dictionary<string, Type> ExtensionMap { get; private set; }
        public static Dictionary<string, ObservableCollection<Content>> ContentDictionary { get; set; }
        
        private static int totalAudio;

        static DataModel()
        {
            // FileNavView Set Up
            ContentDictionary = new Dictionary<string, ObservableCollection<Content>>();
            totalAudio = 0;

            // Initialize what type of content is associated with each file extension
            ExtensionMap = new Dictionary<string, Type>
            {
                //Browser
                { ".pdf", typeof(BrowserContent) },
                { ".png", typeof(BrowserContent) },
                { ".jpg", typeof(BrowserContent) },

                //Audio
                { ".wav", typeof(AudioContent) },
                { ".mp3", typeof(AudioContent) },
                { ".flac", typeof(AudioContent) },

                //Video
                { ".webm", typeof(VideoContent) },
                { ".mkv", typeof(VideoContent) },
                { ".avi", typeof(VideoContent) },
                { ".mpg", typeof(VideoContent) },
                { ".mp4", typeof(VideoContent) }
            };
        }

        public static void InitializeContent()
        {
            ContentDictionary.Clear();
            totalAudio = 0;
            MapDirectoryToContent("DigitalContent");
        }

        public static void MapDirectoryToContent(string relativePath)
        {
            var globalPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;

            string[] directories = Directory.GetDirectories(globalPath + "//" + relativePath);
            string[] files = Directory.GetFiles(globalPath + "//" + relativePath);

            ObservableCollection<Content> contentToAdd = new ObservableCollection<Content>();
            string filePattern = @"[^\\\/]*$";

            // Map content to the current directory.
            foreach (string file in files)
            {
                string extension = Regex.Match(file, @"\.([A-Za-z0-9]+)$").Value;
                Match match = Regex.Match(file, filePattern);
                string filePath = relativePath + "//" + match.Value;
                Content content = CreateContent(filePath, match.Value, extension);
                contentToAdd.Add(content);
            }

            // Map sub directories(FolderContent) to the current directory.
            foreach (string directory in directories)
            {
                Match match = Regex.Match(directory, filePattern);
                FolderContent folderContent = new FolderContent(relativePath + "//" + match.Value, match.Value);
                contentToAdd.Add(folderContent);

                // Recursively map subdirectories to their content.
                MapDirectoryToContent(relativePath + "//" + match.Value);
            }

            ContentDictionary.Add(relativePath, contentToAdd);
        }

        public static Content CreateContent(string path, string name, string extension)
        {
            if (ExtensionMap.ContainsKey(extension))
            {
                Type contentType = ExtensionMap[extension];

                if (contentType == typeof(BrowserContent))
                {
                    return new BrowserContent(path, name);
                }
                else if (contentType == typeof(AudioContent))
                {
                    AudioContent audio = new AudioContent(path, name, totalAudio++);
                    AudioService.Instance.AddSong(audio);
                    return audio;
                }
                else if (contentType == typeof(VideoContent))
                {
                    return new VideoContent(path, name);
                }
            }
            throw new NotSupportedException($"No content type registered for extension {extension}");
        }
    }

    // The Content class determines how a file type/folder is viewed in the application. 
    public abstract class Content
    {
        public string Path { get; protected set; }
        public string Name { get; protected set; }
        public Type SourcePageType { get; protected set; }

        protected Content(string path, string name, Type sourcePageType)
        {
            Path = path;
            Name = name;
            SourcePageType = sourcePageType;
;        }
    }

    public class FolderContent : Content
    {
        public FolderContent(string path, string name) : base(path, name, typeof(FileNavView)) { }
    }

    public class BrowserContent : Content
    {
        public BrowserContent(string path, string name) : base(path, name, typeof(BrowserView)) { }
    }

    public class AudioContent : Content
    {
        public int ID { get; private set; }

        public AudioContent(string path, string name, int id) : base(path, name, typeof(AudioView))
        {
            ID = id;
        }
    }

    public class VideoContent : Content
    {
        public VideoContent(string path, string name) : base(path, name, typeof(VideoView)) { }
    }

}
