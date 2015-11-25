using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLDJConverter.YoutubeDownloaders
{
    // LibVideo is a modern reimplementation of YoutubeExtractor but I'm not sure if
    // the decryption code is working so I'm not going to implement it.

    //public sealed class LibVideoYoutubeDownloader: IYoutubeDownloader
    //{
    //    public static async Task<Result<YoutubeVideo, string>> GetYoutubeVideoInfo(string videoUrl)
    //    {
    //        videoUrl = YoutubeHelpers.NormalizeVideoUrl(videoUrl);
    //        if(string.IsNullOrEmpty(videoUrl))
    //            return "Invalid Link";

    //        try
    //        {
    //            var videos = await YouTubeService.Default.GetAllVideosAsync(link);
    //            var video = videos
    //                .Where(info => info.AudioFormat != AudioFormat.Unknown)
    //                .OrderByDescending(info => info.AudioBitrate)
    //                .First();

    //            return video;
    //        }
    //        catch(WebException e)
    //        {
    //            return (e.Response as HttpWebResponse).StatusCode.ToString();
    //        }
    //        catch(Exception e)
    //        {
    //            return "Private/Removed Video";
    //        }
    //    }
    //}



    //public static async Task<Result<VideoDownload, string>> DownloadYoutubeVideo(Video video)
    //{
    //    // We combine the VideoID with a DateTime hashcode just incase multiple copies
    //    // of the same video are being downloaded.  That way there won't be any file clashes.
    //    string filepath = $"{DateTime.Now.GetHashCode().ToString()}";

    //    try
    //    {
    //        var videoBytes = await video.GetBytesAsync();
    //        File.WriteAllBytes(filepath, videoBytes);
    //        return new VideoDownload(video, filepath);
    //    }
    //    catch(WebException e)
    //    {
    //        return (e.Response as HttpWebResponse).StatusCode.ToString();
    //    }
    //    catch(Exception e)
    //    {
    //        return string.Empty;
    //    }
    //}
}
