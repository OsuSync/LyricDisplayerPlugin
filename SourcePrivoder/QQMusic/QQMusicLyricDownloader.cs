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

    public static class QQMusicLyricDownloader
    {
        public static readonly string API_URL = @"http://c.y.qq.com/lyric/fcgi-bin/fcg_query_lyric.fcg?nobase64=1&musicid={0}&callback=json&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf-8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0";

        public static string LyricsDownload(int id)
        {
            Uri url = new Uri(string.Format(API_URL, id));

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Timeout = 2000;
            request.Referer = "https://y.qq.com/";

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

            return json["lyric"].ToString();
        }
    }
}
