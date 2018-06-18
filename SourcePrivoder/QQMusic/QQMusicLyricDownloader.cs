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

    public class QQMusicLyricDownloader:LyricDownloaderBase
    {
        //public static readonly string API_URL = @"http://c.y.qq.com/lyric/fcgi-bin/fcg_query_lyric.fcg?nobase64=1&musicid={0}&callback=json&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf-8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0&songtype={1}";

        public static readonly string NEW_API_URL = @"https://c.y.qq.com/lyric/fcgi-bin/fcg_query_lyric_new.fcg?g_tk=753738303&songmid={0}&callback=json&songtype={1}";

        public override string DownloadLyric(SearchSongResultBase song, bool request_trans_lyrics)
        {
            string song_type = (song as Song)?.type ?? "0";

            Uri url = new Uri(string.Format(NEW_API_URL,song.ID, song_type));

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);

            request.Timeout = Setting.SearchAndDownloadTimeout;
            request.Referer = "https://y.qq.com/portal/player.html";
            request.Headers.Add("Cookie", "skey=@LVJPZmJUX; p");

            var response = request.GetResponse();

            string content = string.Empty;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                content = reader.ReadToEnd();
            }

            if (content.StartsWith("json("))
            {
                content = content.Remove(0, 5);
            }

            if (content.EndsWith(")"))
            {
                content = content.Remove(content.Length - 1);
            }

            content = System.Web.HttpUtility.HtmlDecode(content);
            JObject json = JObject.Parse(content);

            int result = json["retcode"].ToObject<int>();
            if (result < 0)
                return null;

            content = json[request_trans_lyrics?"trans": "lyric"]?.ToString();
            if (string.IsNullOrWhiteSpace(content))
                return null;

            content = Utils.Base64Decode(content);

            return content;
        }
    }
}
