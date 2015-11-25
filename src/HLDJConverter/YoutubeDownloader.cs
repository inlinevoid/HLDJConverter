using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HLDJConverter
{
    public enum MediaContainer
    {
        Mobile      = 0,
        Flash       = 1,
        Mp4         = 2,
        WebM        = 3,
        Unknown
    }

    public enum AudioCodec
    {
        AAC         = 0,
        Mp3         = 1,
        Vorbis      = 2,
        Unkown,
    }

    public enum AdaptiveType
    {
        None        = 0,
        Audio       = 1,
        Video       = 2,
    }

    public sealed class YoutubeVideo
    {
        public string Title;
        public string Url;
        public List<VideoFormat> Formats;
    }

    public class VideoFormat
    {
        public readonly YoutubeVideo Owner;
        public int FormatCode;
        public int AudioBitrate;
        public int Resolution;
        public string ContainerExtension;
        public string AudioExtension;
        public MediaContainer Container;
        public AudioCodec AudioCodec;
        public AdaptiveType AdaptiveType;
        public string DownloadUrl;
        public bool IsEncrypted;

        public VideoFormat(YoutubeVideo owner)
        {
            Owner = owner;
        }
    }

    public sealed class YoutubeDownloadResult
    {
        public readonly string Filepath;

        public YoutubeDownloadResult(string filepath)
        {
            Filepath = filepath;
        }
    }

    public interface IYoutubeDownloader
    {
        Task<Result<YoutubeVideo, string>> GetYoutubeVideoInfo(string videoUrl);
        Task<Result<YoutubeDownloadResult, string>> DownloadYoutubeVideo(VideoFormat video);
    }
    
    public static class YoutubeHelpers
    {
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

        public static string NormalizeVideoUrl(string videoUrl)
        {
            var id = ExtractYoutubeID(videoUrl);

            if(string.IsNullOrEmpty(id))
                return string.Empty;

            return $"https://www.youtube.com/watch?v={id}";
        }
    }
}
