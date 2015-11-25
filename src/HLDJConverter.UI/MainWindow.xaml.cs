using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using HLDJConverter.UI.Models;
using Newtonsoft.Json;

namespace HLDJConverter.UI
{
    public partial class MainWindow: Window
    {
        public Settings Settings { get; set; }
        public ObservableCollection<ConversionItem> ConversionJobs { get; set; }
        public IYoutubeDownloader YoutubeDownloader;

        public MainWindow()
        {
            // Make sure ffmpeg exists in the same directory.
            if(!File.Exists("ffmpeg.exe"))
            {
                MessageBox.Show(this, "Couldn't start HLDJConverter!  Make sure ffmpeg.exe is in the same directory as HLDJConverter.", "There was a problem");
                Close();
            }

            Settings = new Settings();
            ConversionJobs = new ObservableCollection<ConversionItem>();
            YoutubeDownloader = new YoutubeDownloaders.YTEYoutubeDownloader();

            // Load user config
            if(File.Exists("HLDJConverterConfig.cfg"))
            {
                try
                {
                    Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("HLDJConverterConfig.cfg"));
                }
                catch(Exception e)
                {
                    File.Delete("HLDJConverterConfig.cfg");
                    MessageBox.Show(this, "HLDJConverterConfig.cfg was improperly formatted.  It has been deleted and reset.", "There was a problem");
                }
            }

            Title = $"HLDJConverter {ApplicationHelper.GetVersion()}";
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if(Settings != null)
                File.WriteAllText("HLDJConverterConfig.cfg", JsonConvert.SerializeObject(Settings, Formatting.Indented));

            base.OnClosing(e);
        }
        
        private void InputSectionDragDrop(object sender, DragEventArgs e)
        {
            if(e.Data != null)
                HandleDataObject(e.Data);

            InputSectionDragLeave(sender, e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Handle the "paste" command for easier conversion
            if(e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if(Clipboard.GetDataObject() != null)
                    HandleDataObject(Clipboard.GetDataObject());
            }

            base.OnKeyDown(e);
        }

        private async void HandleDataObject(IDataObject data)
        {
            if(string.IsNullOrEmpty(Settings.OutputFolder))
            {
                
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(this, "You need to set an output folder for your converted songs first.", "There was a problem"));
            }
            else if(data.GetDataPresent(DataFormats.UnicodeText))
            {
                HandleYoutubeConversion((string)data.GetData(DataFormats.UnicodeText));
            }
            else if(data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filepaths = (string[])data.GetData(DataFormats.FileDrop);
                foreach(var filepath in filepaths)
                {
                    string extension = Path.GetExtension(filepath);

                    if(string.IsNullOrWhiteSpace(extension))
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(this, "Folders aren't supported.  Instead, select all of the files inside the folder you wish to convert.", "There was a problem"));
                    }
                    else if(string.Compare(extension, ".url", true) == 0)
                    {
                        HandleYoutubeConversion(YoutubeHelpers.ExtractURLFromShortcut(filepath));
                    }
                    else
                    {
                        HandleFileConversion(filepath);
                    }
                }
            }
        }

        private async void HandleYoutubeConversion(string videoUrl)
        {
            var job = new ConversionItem
            {
                Title = videoUrl
            };

            ConversionJobs.Add(job);
            ConversionJobsListView.ScrollIntoView(job);

            #region New libvideo code.  
            //// Resolve
            //job.Status = ConversionJobStatus.Resolving;
            //var queryResult = await YoutubeDownloader.QueryYoutubeVideo(link);
            //if(queryResult.IsError)
            //{
            //    job.Status = ConversionJobStatus.Error;
            //    job.StatusDetails = queryResult.Error;
            //}
            //else
            //{
            //    var video = queryResult.Value;

            //    // Download
            //    job.Status = ConversionJobStatus.Downloading;
            //    job.Title = $"{video.Title} ({video.Resolution.ToString()}p)";

            //    var downloadResult = await YoutubeDownloader.DownloadYoutubeVideo(video);
            //    if(downloadResult.IsError)
            //    {
            //        job.Status = ConversionJobStatus.Error;
            //        job.StatusDetails = downloadResult.Error;
            //    }
            //    else
            //    {
            //        var download = downloadResult.Value;

            //        // Convert
            //        job.Status = ConversionJobStatus.Converting;
            //        string filename = MediaConverter.RemoveInvalidFilenameCharacters(download.Video.Title);
            //        await MediaConverter.FFmpegConvertToWavAsync(download.Filepath, $"{Settings.OutputFolder}\\{filename}.wav", Settings.OutputBitrate, Settings.OutputVolume);

            //        // Finish
            //        job.Status = ConversionJobStatus.Done;
            //        File.Delete(download.Filepath);
            //    }
            //}
            #endregion 

            // Resolve
            job.Status = ConversionJobStatus.Resolving;
            var queryResult = await YoutubeDownloader.GetYoutubeVideoInfo(videoUrl);
            if(queryResult.IsError)
            {
                job.Status = ConversionJobStatus.Error;
                job.StatusDetails = queryResult.Error;
            }
            else
            {
                // Download
                job.Status = ConversionJobStatus.Downloading;
                var video = queryResult.Value;
                var highestQualityVersion = video.Formats
                    .Where(v => v.AudioCodec != AudioCodec.Unkown)
                    .OrderByDescending(info => info.Resolution)
                    .First();
                job.Title = $"YouTube - {video.Title}";

                var downloadResult = await YoutubeDownloader.DownloadYoutubeVideo(highestQualityVersion);
                if(downloadResult.IsError)
                {
                    job.Status = ConversionJobStatus.Error;
                    job.StatusDetails = downloadResult.Error;
                }
                else
                {
                    // Convert
                    job.Status = ConversionJobStatus.Converting;
                    var download = downloadResult.Value;
                    string filename = MediaConverter.RemoveInvalidFilenameCharacters(video.Title);
                    string dstFilepath = MediaConverter.EnsureUniqueFilepath($"{Settings.OutputFolder}\\{filename}.wav");
                    await MediaConverter.FFmpegConvertToWavAsync(download.Filepath, dstFilepath, Settings.OutputFrequency, Settings.OutputVolumeMultiplier);

                    // Finish
                    job.Status = ConversionJobStatus.Done;
                    File.Delete(download.Filepath);
                }
            }
        }
        
        private async void HandleFileConversion(string filepath)
        {
            //Path.get
            string filename = Path.GetFileNameWithoutExtension(filepath);

            var job = new ConversionItem
            {
                Title = filename,
            };
            ConversionJobs.Add(job);
            ConversionJobsListView.ScrollIntoView(job);

            // Convert
            job.Status = ConversionJobStatus.Converting;
            filename = MediaConverter.RemoveInvalidFilenameCharacters(filename);
            string dstFilepath = MediaConverter.EnsureUniqueFilepath($"{Settings.OutputFolder}\\{filename}.wav");
            await MediaConverter.FFmpegConvertToWavAsync(filepath, dstFilepath, Settings.OutputFrequency, Settings.OutputVolumeMultiplier);

            // Finish
            job.Status = ConversionJobStatus.Done;
        }

        private void InputSectionDragEnter(object sender, DragEventArgs e)
        {
            (sender as Grid).Opacity = 0.75;
        }

        private void InputSectionDragLeave(object sender, DragEventArgs e)
        {
            (sender as Grid).Opacity = 0.5;
        }

        private void OutputFolderDialogClick(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.ShowDialog();

            if(!string.IsNullOrEmpty(dialog.SelectedPath))
                Settings.OutputFolder = dialog.SelectedPath;
        }

        private void HyperlinkClick(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
