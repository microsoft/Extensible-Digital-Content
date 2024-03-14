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
    /// <summary>
    /// The DataModel class is responsible for mapping the files in the DigitalContent folder to
    /// the correct pages in the app.
    /// </summary>
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

        /// <summary>
        /// Recursively maps the files and directories in the DigitalContent folder to their corresponding content.
        /// </summary>
        public static void MapDirectoryToContent(string relativePath)
        {
            var globalPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;

            string[] directories = Directory.GetDirectories(globalPath + "//" + relativePath);
            string[] files = Directory.GetFiles(globalPath + "//" + relativePath);

            ObservableCollection<Content> contentToAdd = new ObservableCollection<Content>();
            string filePattern = @"[^\\\/]*$";

            // Map files to content in the current directory.
            foreach (string file in files)
            {
                string extension = Regex.Match(file, @"\.([A-Za-z0-9]+)$").Value;
                Match match = Regex.Match(file, filePattern);
                string filePath = relativePath + "//" + match.Value;
                Content content = CreateContent(filePath, match.Value, extension);
                contentToAdd.Add(content);
            }

            // Map subdirectories into FolderContent.
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

        /// <summary>
        /// Takes a file path, name, and extension to initialize and return content.
        /// </summary>
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

    /// <summary>
    /// The content class is the base class for all content types in the app.
    /// To support a new type of content/file, you can.
    /// 1. Create a new class that inherits Content.
    /// 2. Add the file extensions you want to use with your content to the ExtensionMap in the DataModel constructor.
    /// 3. Set up content initialization in the CreateContent function.
    /// </summary>
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
