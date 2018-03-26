using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder.Kugou
{
    public class KugouLyricDownloader:LyricDownloaderBase
    {
        public static readonly string API_URL = @"http://www.kugou.com/yy/index.php?r=play/getdata&hash={0}";

        public override string DownloadLyric(SearchSongResultBase song, bool request_trans_lyrics)
        {
            //没支持翻译歌词的
            if (request_trans_lyrics)
                return string.Empty;

            Uri url = new Uri(string.Format(API_URL, song.ID));

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Timeout = 2000;

            var response = request.GetResponse();

            string content = string.Empty;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                content = reader.ReadToEnd();
            }

            JObject obj = JObject.Parse(content);
            if ((int)obj["err_code"] != 0)
                return null;
            var raw_lyric = obj["data"]["lyrics"].ToString();
            var lyrics = raw_lyric.Replace("\r\n", "\n");

            return lyrics;
        }
    }
}
