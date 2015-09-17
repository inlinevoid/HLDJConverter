using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HLDJConverter
{
    public static class Gist
    {
        public static GistSubmissionResponse CreateAnonymousGist(string description, bool isPublic, string contentFilename, string content)
        {
            var submission = new GistSubmission
            {
                Description = description,
                IsPublic = isPublic,
                Files = new Dictionary<string, GistFile>
                {
                    [contentFilename] = new GistFile {Content = content}, 
                },
            };

            var request = (HttpWebRequest)WebRequest.Create("https://api.github.com/gists");
            request.ContentType = "application/json";
            request.Method = "POST";
            request.UserAgent = "HLDJConverter";

            using(var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(JsonConvert.SerializeObject(submission));
            }

            var response = (HttpWebResponse)request.GetResponse();
            using(var reader = new StreamReader(response.GetResponseStream()))
            {
                return JsonConvert.DeserializeObject<GistSubmissionResponse>(reader.ReadToEnd());
            }
        }
    }

    public sealed class GistSubmission
    {
        [JsonProperty("description")]
        public string Description {get; set; }

        [JsonProperty("public")]
        public bool IsPublic {get; set; }

        [JsonProperty("files")]
        public Dictionary<string, GistFile> Files {get; set; }
    }

    public sealed class GistFile
    {
        [JsonProperty("content")]
        public string Content {get; set; }
    }

    public sealed class GistSubmissionResponse
    {
        [JsonProperty("html_url")]
        public string Url {get; set; }
    }
}
