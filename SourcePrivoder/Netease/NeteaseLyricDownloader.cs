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
    public class NeteaseLyricDownloader:LyricDownloaderBase
    {
        //tv=-1 是翻译版本的歌词
        //lv=1 是源版本歌词
        private static readonly string LYRIC_API_URL = "https://music.163.com/api/song/lyric?id={0}&{1}";

        public override string DownloadLyric(SearchSongResultBase song, bool request_trans_lyrics)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(string.Format(LYRIC_API_URL,song.ID, request_trans_lyrics ? "tv=-1":"lv=1"));
            request.Timeout = Setting.SearchAndDownloadTimeout;

            var response = request.GetResponse();

            string content = string.Empty;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                content = reader.ReadToEnd();
            }

            JObject json = JObject.Parse(content);

            return json[request_trans_lyrics ? "tlyric": "lrc"]["lyric"].ToString();
        }
    }
}
