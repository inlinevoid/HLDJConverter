using System;
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

        public MainWindow()
        {
            // Make sure ffmpeg exists in the same directory.
            if(!File.Exists("ffmpeg.exe"))
            {
                MessageBox.Show(this, "Couldn't start HLDJConverter!  ffmpeg.exe is missing from the directory.", "There was a problem");
                Close();
            }

            Settings = new Settings();
            ConversionJobs = new ObservableCollection<ConversionItem>();

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

            Title = $"HLDJConverter v{ApplicationHelper.GetVersion()}";
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
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(this, "You haven't set an output folder yet.", "There was a problem"));
            }
            else if(data.GetDataPresent(DataFormats.UnicodeText))
            {
                HandleYoutubeConversion((string)data.GetData(DataFormats.UnicodeText));
            }
            else if(data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])data.GetData(DataFormats.FileDrop);
                foreach(var file in files)
                {
                    if(string.Compare(Path.GetExtension(file), ".url", true) == 0)
                    {
                        HandleYoutubeConversion(YoutubeDownloader.ExtractURLFromShortcut(file));
                    }
                    else
                    {
                        HandleFileConversion(file);
                    }
                }
            }
        }


        private async void HandleYoutubeConversion(string link)
        {
            var job = new ConversionItem
            {
                Title = link
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
            var queryResult = await YoutubeDownloader.QueryYoutubeVideoOld(link);
            if(queryResult.IsError)
            {
                job.Status = ConversionJobStatus.Error;
                job.StatusDetails = queryResult.Error;
            }
            else
            {
                var video = queryResult.Value;

                // Download
                job.Status = ConversionJobStatus.Downloading;
                job.Title = $"{video.Title} ({video.Resolution.ToString()}p)";

                var downloadResult = await YoutubeDownloader.DownloadYoutubeVideoOld(video);
                if(downloadResult.IsError)
                {
                    job.Status = ConversionJobStatus.Error;
                    job.StatusDetails = downloadResult.Error;
                }
                else
                {
                    var download = downloadResult.Value;

                    // Convert
                    job.Status = ConversionJobStatus.Converting;
                    string filename = MediaConverter.RemoveInvalidFilenameCharacters(video.Title);
                    string dstFilepath = MediaConverter.EnsureUniqueFilepath($"{Settings.OutputFolder}\\{filename}.wav");
                    await MediaConverter.FFmpegConvertToWavAsync(download.Filepath, dstFilepath, Settings.OutputBitrate, Settings.OutputVolume);

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
            await MediaConverter.FFmpegConvertToWavAsync(filepath, dstFilepath, Settings.OutputBitrate, Settings.OutputVolume);

            // Finish
            job.Status = ConversionJobStatus.Done;
        }

        //private async Task HandleYoutubeConvert(string link)
        //{
        //    var item = new ConversionItem
        //    {
        //        Title = link,
        //    };

        //    var video = await ConversionJobDownload(item, link);
        //    await ConversionJobConvert(item, video.Filepath, $"{Settings.OutputFolder}\\{video.Video.Title}.wav");

        //    item.Status = ConversionJobStatus.Done;
        //}

        //private async Task ConversionJobConvert(ConversionItem item, string srcFilepath, string dstFilepath)
        //{
        //    item.Status = ConversionJobStatus.Converting;
        //    await MediaConverter.FFmpegConvertToWavAsync(srcFilepath, dstFilepath, Settings.OutputBitrate, Settings.OutputVolume);
        //}

        //private async Task<VideoDownload> ConversionJobDownload(ConversionItem item, string link)
        //{
        //    item.Status = ConversionJobStatus.Resolving;
            
        //    // Resolve
        //    var queryResult = await YoutubeDownloader.QueryYoutubeVideo(link);
        //    if(queryResult.IsError)
        //    {
        //        item.Status = ConversionJobStatus.Error;
        //        item.StatusDetails = queryResult.Error;
        //    }
        //    else
        //    {
        //        var video = queryResult.Value;

        //        // Download
        //        item.Status = ConversionJobStatus.Downloading;
        //        item.Title = $"{video.Title} ({video.Resolution.ToString()}p)";

        //        var downloadResult = await YoutubeDownloader.DownloadYoutubeVideo(video);
        //        if(downloadResult.IsError)
        //        {
        //            item.Status = ConversionJobStatus.Error;
        //            item.StatusDetails = downloadResult.Error;
        //        }

        //        downloadResult.Value.
        //    }
        //}


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
