using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder.QQMusic
{
    public class QQMusicSourceProvider : SourceProviderBase
    {
        public override Lyrics ProvideLyric(string artist, string title, int time)
        {
            var search_result = QQMusicSearch.Search(artist, title).Result;

#if DEBUG
            foreach (var r in search_result)
            {
                Utils.Debug($"music_id:{r.id} artist:{r.artist} title:{r.name} time{r.interval*1000}({Math.Abs(r.interval*1000 - time):F2})");
            }
#endif

            var fuck_value = 1000;

            search_result.RemoveAll((r) => Math.Abs(r.interval*1000 - time) > fuck_value);

            string check_Str = $"{artist}{title}";

            search_result.Sort((a, b) => _GetEditDistance(a) - _GetEditDistance(b));

            if (search_result.Count == 0)
            {
                return null;
            }

            var result = search_result.First();

            Utils.Debug($"Picked music_id:{result.id} artist:{result.artist} title:{result.name}");

            var lyric_cont = QQMusicLyricDownloader.LyricsDownload(result.id);

            if (string.IsNullOrWhiteSpace(lyric_cont))
            {
                return null;
            }

            return QQMusicLyricParser.Parse(lyric_cont);

            int _GetEditDistance(Song s) => Utils.EditDistance($"{s.artist}{s.name}", check_Str);
        }
    }
}
