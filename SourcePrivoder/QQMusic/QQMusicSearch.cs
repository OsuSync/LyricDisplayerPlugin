using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder.QQMusic
{
    #region JSON

    public struct Singer
    {
        public string name { get; set; }
        public string title { get; set; }
    }

    public struct Song
    {
        public List<Singer> singer { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public int id { get; set; }

        //建议qq音乐那边声明这个玩意的人跟我一起重学英语,谢谢
        /// <summary>
        /// duration
        /// </summary>
        public int interval { get; set; }

        public string artist { get => singer?.First().name ?? null; }

        public override string ToString()
        {
            return $"({id}){artist} - {title} ({interval / 60}:{interval % 60})";
        }
    }

    #endregion

    public static class QQMusicSearch
    {
        private static readonly string API_URL = @"http://c.y.qq.com/soso/fcgi-bin/client_search_cp?ct=24&qqmusic_ver=1298&new_json=1&remoteplace=txt.yqq.song&t=0&aggr=1&cr=1&catZhida=1&lossless=0&flag_qc=0&p=1&n=20&w={0} {1}&g_tk=5381&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0";

        public static Task<List<Song>> Search(string title, string artist)
        {
            return Task.Run<List<Song>>(
                () =>
                {
                    Uri url = new Uri(string.Format(API_URL, artist, title));

                    HttpWebRequest request = HttpWebRequest.CreateHttp(url);
                    request.Timeout = 2000;

                    var response = request.GetResponse();

                    string content = string.Empty;

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                    }

                    var json = JObject.Parse(content);

                    List<Song> songs = (json["data"]["song"]["list"].ToObject<List<Song>>());

                    return songs;
                }
                );
        }
    }

}
