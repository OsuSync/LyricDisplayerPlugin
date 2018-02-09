using LyricDisplayerPlugin.SourcePrivoder.Netease;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin
{
    public class NeteaseSourceProvider:SourceProviderBase
    {
        public override Lyrics ProvideLyric(string artist,string title,int time)
        {
            var search_result = NeteaseSearch.Search(artist, title).Result;

            search_result.RemoveAll((r) => Math.Abs(r.duration-time)<time*0.1f);

            search_result.Sort((a,b)=> (a.duration - time)-(b.duration - time));

            var result = search_result.First();

            var lyric_cont = NeteaseLyricDownloader.DownloadLyricFromNetease(result.id);

            if (string.IsNullOrWhiteSpace(lyric_cont))
            {
                return null;
            }

            return NeteaseLyricParser.Parse(lyric_cont);
        }
    }
}
