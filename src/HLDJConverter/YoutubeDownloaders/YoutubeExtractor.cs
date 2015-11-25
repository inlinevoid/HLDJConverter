using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace HLDJConverter.YoutubeDownloaders
{
    public sealed class YTEVideoFormat: VideoFormat
    {
        public VideoInfo YTEVideoInfo;

        public YTEVideoFormat(YoutubeVideo owner): base(owner) { }
    }

    public sealed class YTEYoutubeDownloader: IYoutubeDownloader
    {
        public async Task<Result<YoutubeVideo, string>> GetYoutubeVideoInfo(string videoUrl)
        {
            videoUrl = YoutubeHelpers.NormalizeVideoUrl(videoUrl);
            if(string.IsNullOrEmpty(videoUrl))
                return "Invalid Link";
            
            try
            {
                var videos = await Task.Run(() => DownloadUrlResolver.GetDownloadUrls(videoUrl));

                var video = new YoutubeVideo()
                {
                    Title = videos.First().Title,
                    Url = videoUrl,
                };

                video.Formats = videos.Select(v => new YTEVideoFormat(video)
                {
                    FormatCode = v.FormatCode,
                    AudioBitrate = v.AudioBitrate,
                    Resolution = v.Resolution,
                    ContainerExtension = v.VideoExtension,
                    AudioExtension = v.AudioExtension,
                    Container = (MediaContainer)v.VideoType,
                    AudioCodec = (AudioCodec)v.AudioType,
                    AdaptiveType = (AdaptiveType)v.AdaptiveType,
                    DownloadUrl = v.DownloadUrl,
                    IsEncrypted = v.RequiresDecryption,
                    YTEVideoInfo = v,
                } as VideoFormat).ToList();
                
                return video;
            }
            catch(WebException e)
            {
                return (e.Response as HttpWebResponse).StatusCode.ToString();
            }
            catch(Exception)
            {
                return "Private/Removed Video";
            }
        }

        public async Task<Result<YoutubeDownloadResult, string>> DownloadYoutubeVideo(VideoFormat video)
        {
            return await DownloadYoutubeVideo(video as YTEVideoFormat);
        }

        public async Task<Result<YoutubeDownloadResult, string>> DownloadYoutubeVideo(YTEVideoFormat video)
        {
            if(video == null)
                throw new Exception("null video passed to DownloadYoutubeVideo");

            // We combine the VideoID with a DateTime hashcode just incase multiple copies
            // of the same video are being downloaded.  That way there won't be any file clashes.
            string filepath = $"{Guid.NewGuid().ToString()}.temp";

            try
            {
                if(video.YTEVideoInfo.RequiresDecryption)
                    await Task.Run(() => DownloadUrlResolver.DecryptDownloadUrl(video.YTEVideoInfo));
                
                var downloader = new VideoDownloader(video.YTEVideoInfo, filepath);
                await Task.Run(() => downloader.Execute());

                return new YoutubeDownloadResult(filepath);
            }
            catch(WebException e)
            {
                return (e.Response as HttpWebResponse).StatusCode.ToString();
            }
            catch(Exception)
            {
                return string.Empty;
            }
        }
    }
}
