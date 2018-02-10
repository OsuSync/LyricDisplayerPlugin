using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder.Netease
{
    public class NeteaseSearch:SongSearchBase
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

        public class Song:SearchSongResultBase
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

            public override int Duration => duration;

            public override string ID =>id.ToString();

            public override string Title => name;

            public override string Artist =>artists?.First().name;
        }

        #endregion

        private static readonly string API_URL = "http://music.163.com/api/search/get/";
        private static readonly int SEARCH_LIMIT = 5;

        public override Task<List<SearchSongResultBase>> Search(params string[] param_arr)
        {
            return Task.Run(
                () =>
                {
                    string title = param_arr[0], artist = param_arr[1];
                    Uri url = new Uri($"{API_URL}?s={artist} {title}&limit={SEARCH_LIMIT}&type=1&offset=0");

                    HttpWebRequest request = HttpWebRequest.CreateHttp(url);
                    request.Method = "POST";
                    request.Timeout = 2000;
                    request.Referer = "http://music.163.com";
                    request.Headers["appver"] = $"2.0.2";

                    var response = request.GetResponse();

                    string content = string.Empty;

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                    }

                    JObject json = JObject.Parse(content);

                    var count = json["result"]["songCount"]?.ToObject<int>();

                    if (count==0)
                    {
                        return new List<SearchSongResultBase>();
                    }

                    var result = json["result"]["songs"].ToObject<List<Song>>().ToList<SearchSongResultBase>();

                    return result;
                }
                );
        }
    }
}
