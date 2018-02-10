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

            search_result.RemoveAll((r) => Math.Abs(r.duration-time)>time*0.1f);

            string check_Str = $"{artist}{title}";

            search_result.Sort((a,b)=>_GetEditDistance(a)-_GetEditDistance(b));

#if DEBUG
            foreach (var r in search_result)
            {
                Utils.Debug($"music_id:{r.id} artist:{r.artists.First().name} title:{r.name} time{r.duration}({Math.Abs(r.duration - time) > time * 0.1f :F2})");
            }
#endif

            var result = search_result.First();

            Utils.Debug($"Picked music_id:{result.id} artist:{result.artists.First().name} title:{result.name}");

            var lyric_cont = NeteaseLyricDownloader.DownloadLyricFromNetease(result.id);

            if (string.IsNullOrWhiteSpace(lyric_cont))
            {
                return null;
            }

            return NeteaseLyricParser.Parse(lyric_cont);

            int _GetEditDistance(NeteaseSearch.Song s) => Utils.EditDistance($"{s.artists.First().name}{s.name}", check_Str);
        }
    }
}
