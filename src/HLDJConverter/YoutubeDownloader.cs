using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VideoLibrary;

namespace HLDJConverter
{
    public sealed class VideoDownload
    {
        public readonly Video Video;
        public readonly string Filepath;

        public VideoDownload(Video video, string filepath)
        {
            Video = video;
            Filepath = filepath;
        }
    }

    public sealed class YoutubeDownloadResult
    {
        public string Filepath;
    }

    //public sealed class YoutubeQueryResult
    //{
    //    public YoutubeExtractor.VideoInfo Video;
    //    public string VideoID;
    //}

    public sealed class YoutubeDownloader
    {
        public static async Task<Result<YoutubeExtractor.VideoInfo, string>> QueryYoutubeVideoOld(string link)
        {
            var id = ExtractYoutubeID(link);
            if(string.IsNullOrEmpty(id))
                return "Invalid Link";
            else
                link = $"https://www.youtube.com/watch?v={id}";
            
            try
            {
                var videos = await Task.Run(() => YoutubeExtractor.DownloadUrlResolver.GetDownloadUrls(link));
                var video = videos
                    .Where(info => info.AudioType != YoutubeExtractor.AudioType.Unknown)
                    .OrderByDescending(info => info.AudioBitrate)
                    .First();

                if(video.RequiresDecryption)
                    await Task.Run(() => YoutubeExtractor.DownloadUrlResolver.DecryptDownloadUrl(video));

                return video;
            }
            catch(WebException e)
            {
                return (e.Response as HttpWebResponse).StatusCode.ToString();
            }
            catch(Exception e)
            {
                return "Private/Removed Video";
            }
        }

        public static async Task<Result<YoutubeDownloadResult, string>> DownloadYoutubeVideoOld(YoutubeExtractor.VideoInfo video)
        {
            // We combine the VideoID with a DateTime hashcode just incase multiple copies
            // of the same video are being downloaded.  That way there won't be any file clashes.
            string filepath = $"{Guid.NewGuid().ToString()}.temp";

            try
            {
                var downloader = new YoutubeExtractor.VideoDownloader(video, filepath);
                await downloader.ExecuteAsync();
                return new YoutubeDownloadResult
                {
                    Filepath = filepath,
                };
            }
            catch(WebException e)
            {
                return (e.Response as HttpWebResponse).StatusCode.ToString();
            }
            catch(Exception e)
            {
                return string.Empty;
            }
        }

        public static async Task<Result<Video, string>> QueryYoutubeVideo(string link)
        {
            // Run the link through a parser that can extract the youtube ID from various
            // versions of valid youtube links.
            var id = ExtractYoutubeID(link);
            if(string.IsNullOrEmpty(id))
                return "Invalid Link";
            else
                link = $"https://www.youtube.com/watch?v={id}";
            
            try
            {
                var videos = await YouTubeService.Default.GetAllVideosAsync(link);
                var video = videos
                    .Where(info => info.AudioFormat != AudioFormat.Unknown)
                    .OrderByDescending(info => info.AudioBitrate)
                    .First();

                return video;
            }
            catch(WebException e)
            {
                return (e.Response as HttpWebResponse).StatusCode.ToString();
            }
            catch(Exception e)
            {
                return "Private/Removed Video";
            }
        }

        public static async Task<Result<VideoDownload, string>> DownloadYoutubeVideo(Video video)
        {
            // We combine the VideoID with a DateTime hashcode just incase multiple copies
            // of the same video are being downloaded.  That way there won't be any file clashes.
            string filepath = $"{DateTime.Now.GetHashCode().ToString()}";

            try
            {
                var videoBytes = await video.GetBytesAsync();
                File.WriteAllBytes(filepath, videoBytes);
                return new VideoDownload(video, filepath);
            }
            catch(WebException e)
            {
                return (e.Response as HttpWebResponse).StatusCode.ToString();
            }
            catch(Exception e)
            {
                return string.Empty;
            }
        }

        public static string ExtractYoutubeID(string link)
        {
            return Regex.Match(link, @"^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*").Groups?[2]?.Value;
        }

        public static string ExtractURLFromShortcut(string filepath)
        {
            var match = Regex.Match(File.ReadAllText(filepath), "URL=(.*?)\r\n");

            if(match == null)
                return null;

            return match.Groups[1].Value;
        }
    }
}
