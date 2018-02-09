using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder.Netease
{
    public static class NeteaseSearch
    {
        #region Search Result
        public class Artist
        {
            public List<string> alias { get; set; }
            public string picUrl { get; set; }
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Album
        {
            public int status { get; set; }
            public int copyrightId { get; set; }
            public string name { get; set; }
            public Artist artist { get; set; }
            public ulong publishTime { get; set; }
            public int id { get; set; }
            public int size { get; set; }
        }

        public class Song
        {
            public Album album { get; set; }
            public int status { get; set; }
            public int copyrightId { get; set; }
            public string name { get; set; }
            public int mvid { get; set; }
            public List<string> Alias { get; set; }
            public List<Artist> artists { get; set; }
            public int duration { get; set; }
            public int id { get; set; }
        }

        public class Result
        {
            public List<Song> songs { get; set; }
        }

        public class SearchResult
        {
            public Result result { get; set; }
        }

        #endregion

        private static readonly string API_URL = "http://music.163.com/api/search/get/";
        private static readonly int SEARCH_LIMIT = 5;

        public static Task<List<Song>> Search(string title, string artist)
        {
            return Task.Run<List<Song>>(
                () =>
                {
                    Uri url = new Uri($"{API_URL}?s={title}&limit={SEARCH_LIMIT}&type=1&offset=0");

                    HttpWebRequest request = HttpWebRequest.CreateHttp(url);
                    request.Method = "POST";
                    request.Referer = "http://music.163.com";
                    request.Headers["appver"] = $"2.0.2";

                    var response = request.GetResponse();

                    string content = string.Empty;

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                    }

                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResult>(content);

                    return result.result.songs;
                }
                );
        }
    }
}
