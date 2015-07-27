using System;
using System.Collections.Generic;
using System.Linq;
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
        public static YoutubeQueryResult QueryYoutubevideo(string link)
        {
            string videoID = ExtractYoutubeID(link);
            var videoInfos = DownloadUrlResolver.GetDownloadUrls($"https://www.youtube.com/watch?v={videoID}");
            
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
            string filename = query.VideoID + DateTime.Now.GetHashCode().ToString();

            var downloader = new VideoDownloader(query.Video, $"{filename}");
            downloader.Execute();

            return new YoutubeDownloadResult
            {
                Filepath = filename,
                VideoTitle = query.Video.Title,
            };
        }

        public static string ExtractYoutubeID(string link)
        {
            var match = Regex.Match(link, @"^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*");

            if(match == null)
                return null;

            return match.Groups[2].Value;
        }
    }
}
