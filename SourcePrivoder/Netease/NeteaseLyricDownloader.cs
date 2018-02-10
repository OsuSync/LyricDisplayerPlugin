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
        private static readonly string LYRIC_RAW_API_URL = @"http://music.163.com/api/song/media?id=";

        public override string DownloadLyric(SearchSongResultBase song)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(LYRIC_RAW_API_URL + song.ID);

            var response = request.GetResponse();

            string content = string.Empty;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                content = reader.ReadToEnd();
            }

            JObject json = JObject.Parse(content);

            return json["lyric"].ToString();
        }
    }
}
