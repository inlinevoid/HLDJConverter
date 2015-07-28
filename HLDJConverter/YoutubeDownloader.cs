using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace HLDJConverter
{
    public sealed class YoutubeDownloadResult
    {
        public string Filepath;
        public string VideoTitle;
        public bool IsSuccess;
        public string ErrorMessage;
    }

    public sealed class YoutubeQueryResult
    {
        public VideoInfo Video;
        public string VideoID;
        public bool IsSuccess;
        public string ErrorMessage;
    }

    public sealed class YoutubeDownloader
    {
        public static YoutubeQueryResult QueryYoutubeVideo(string link)
        {
            string videoID = ExtractYoutubeID(link);

            IEnumerable<VideoInfo> videoInfos = null;
            try
            {
                videoInfos = DownloadUrlResolver.GetDownloadUrls($"https://www.youtube.com/watch?v={videoID}");
            }
            catch(WebException e)
            {
                var response = (e.Response as HttpWebResponse);
                return new YoutubeQueryResult
                {
                    Video = null,
                    VideoID = videoID,
                    IsSuccess = false,
                    ErrorMessage = response.StatusCode.ToString()
                };
            }
            catch(YoutubeParseException e)
            {
                return new YoutubeQueryResult
                {
                    Video = null,
                    VideoID = videoID,
                    IsSuccess = false,
                    ErrorMessage = "Private/Removed Video",
                };
            }
            
            var video = videoInfos
                .Where(info => info.AudioType != AudioType.Unknown)
                .OrderByDescending(info => info.Resolution)
                .First();
            

            if(video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            return new YoutubeQueryResult
            {
                Video = video,
                VideoID = videoID,
                IsSuccess = true,
                ErrorMessage = "",
            };
        }

        public static YoutubeDownloadResult DownloadYoutubeVideo(YoutubeQueryResult query)
        {
            // We combine the VideoID with a DateTime hashcode just incase multiple copies
            // of the same video are being downloaded.  That way there won't be any file clashes.
            string filename = $"{query.VideoID}{DateTime.Now.GetHashCode().ToString()}";
            var downloader = new VideoDownloader(query.Video, $"{filename}");
            
            try
            {
                downloader.Execute();
            }
            catch(WebException e)
            {
                var response = e.Response as HttpWebResponse;
                return new YoutubeDownloadResult
                {
                    Filepath = filename,
                    VideoTitle = query.Video.Title,
                    IsSuccess = false,
                    ErrorMessage = response.StatusCode.ToString(),
                };
            }

            return new YoutubeDownloadResult
            {
                Filepath = filename,
                VideoTitle = query.Video.Title,
                IsSuccess = true,
                ErrorMessage = "",
            };
        }

        public static string ExtractYoutubeID(string link)
        {
            var match = Regex.Match(link, @"^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*");

            if(match == null)
                return null;

            return match.Groups[2].Value;
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
