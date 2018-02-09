using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder.Netease
{
    class NeteaseLyricDownloader
    {
        public class Result
        {
            public string lyric { get; set; }
        }
        private static readonly string LYRIC_RAW_API_URL = @"http://music.163.com/api/song/media?id=";

        public static string DownloadLyricFromNetease(int music_id)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(LYRIC_RAW_API_URL + music_id);

            var response = request.GetResponse();

            string content = string.Empty;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                content = reader.ReadToEnd();
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Result>(content).lyric;
        }
    }
}
