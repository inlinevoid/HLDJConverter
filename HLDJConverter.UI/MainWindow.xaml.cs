using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Newtonsoft.Json;

namespace HLDJConverter.UI
{
    public partial class MainWindow: Window
    {
        public Settings Settings { get; set; }
        public ObservableCollection<ConversionJob> ConversionJobs { get; set; }

        public MainWindow()
        {
            if(!File.Exists("HLDJConverterConfig.cfg"))
            {
                Settings = new Settings()
                {
                    OutputFolder = "",
                };
            }
            else
            {
                Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("HLDJConverterConfig.cfg"));
            }

            ConversionJobs = new ObservableCollection<ConversionJob>();
            InitializeComponent();

            // Make sure ffmpeg is in the directory.
            if(!File.Exists("ffmpeg.exe"))
            {
                MessageBox.Show("Couldn't start HLDJConverter!  FFmpeg.exe is missing from the directory.");
                Close();
            }
        }
        

        private void InputSectionDragDrop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.Text))
            {
                Application.Current.Dispatcher.BeginInvoke(
                    new Func<Task>(async () => await HandleYoutubeConversion((string)e.Data.GetData(DataFormats.Text))));
            }
            else if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach(var file in files)
                {
                    if(Path.GetExtension(file) == ".url")
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                            new Func<Task>(async () => await HandleYoutubeConversion(YoutubeDownloader.ExtractURLFromShortcut(file))));
                    }
                    else
                    {
                        Application.Current.Dispatcher.BeginInvoke(
                            new Func<Task>(async () => await HandleFileConversion(file)));
                    }
                }
            }

            InputSectionDragLeave(sender, e);
        }

        private void InputSectionDragEnter(object sender, DragEventArgs e)
        {
            (sender as Grid).Opacity = 0.75;
        }

        private void InputSectionDragLeave(object sender, DragEventArgs e)
        {
            (sender as Grid).Opacity = 0.5;
        }

        private async Task HandleYoutubeConversion(string link)
        {
            var job = new ConversionJob
            {
                DisplayName = link,
                Status = "Resolving",
            };
            ConversionJobs.Add(job);
            ConversionJobsListView.ScrollIntoView(job);

            // Query youtube for the highest quality video download
            var query = await Task.Run(() => YoutubeDownloader.QueryYoutubeVideo(link));

            if(!query.IsSuccess)
            {
                job.Status = $"Error: {query.ErrorMessage}";
                return;
            }

            // Download
            job.DisplayName = $"{query.Video.Title} ({query.Video.Resolution.ToString()}p)";
            job.Status = "Downloading";
            var result = await Task.Run(() => YoutubeDownloader.DownloadYoutubeVideo(query));

            if(!result.IsSuccess)
            {
                job.Status = $"Error: {result.ErrorMessage}";
                File.Delete(result.Filepath);
                return;
            }

            // Convert
            job.Status = "Converting";
            await FFmpegConverter.ConvertToWavAsync(result.Filepath, Settings.OutputFolder, result.VideoTitle);

            // Finish
            job.Status = "Done";
            File.Delete(result.Filepath);
        }

        private async Task HandleFileConversion(string filepath)
        {
            var job = new ConversionJob
            {
                DisplayName = Path.GetFileNameWithoutExtension(filepath),
                Status = "Converting",
            };
            ConversionJobs.Add(job);
            ConversionJobsListView.ScrollIntoView(job);

            // Convert
            await FFmpegConverter.ConvertToWavAsync(filepath, Settings.OutputFolder);

            // Finish
            job.Status = "Done";
        }
        
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File.WriteAllText("HLDJConverterConfig.cfg", JsonConvert.SerializeObject(Settings, Formatting.Indented));
        }

        private void OutputFolderDialogClick(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            dialog.ShowDialog();

            if(!string.IsNullOrEmpty(dialog.SelectedPath))
                Settings.OutputFolder = dialog.SelectedPath;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:inlinevoidmain@gmail.com");
        }

        private void HelpRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
