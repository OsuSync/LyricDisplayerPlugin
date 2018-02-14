using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder.Kugou
{
    public class KugouSearchResultSong : SearchSongResultBase
    {
        public int duration { get; set; }
        public string singername { get; set; }
        public string songname { get; set; }
        //public int audio_id { get; set; }
        
        /// <summary>
        /// 获取歌词需要这个玩意,所以拿着个当ID吧
        /// </summary>
        public string hash { get; set; }

        public override string Title => songname;
        public override string Artist => singername;
        public override int Duration => duration*1000;
        public override string ID => hash;
    }

    public class KugouSearcher : SongSearchBase<KugouSearchResultSong>
    {
        static readonly string API_URL = @"http://mobilecdn.kugou.com/api/v3/search/song?format=json&keyword={1} {0}&page=1&pagesize=20&showtype=1";

        public override List<KugouSearchResultSong> Search(params string[] param_arr)
        {
            string title = param_arr[0], artist = param_arr[1];
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

            if (!string.IsNullOrWhiteSpace(json["error"].ToString()))
                return new List<KugouSearchResultSong>();

            return json["data"]["info"].ToObject<List<KugouSearchResultSong>>();
        }
    }
}
