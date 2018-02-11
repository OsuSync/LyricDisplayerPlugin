using LyricDisplayerPlugin.SourcePrivoder;
using LyricDisplayerPlugin.SourcePrivoder.QQMusic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin
{
    public abstract class SourceProviderBase
    {
        public abstract Lyrics ProvideLyric(string artist, string title, int time);
    }

    public abstract class SourceProviderBase<SEARCHRESULT,SEARCHER,DOWNLOADER,PARSER>:SourceProviderBase where DOWNLOADER:LyricDownloaderBase,new() where PARSER:LyricParserBase,new() where SEARCHER:SongSearchBase<SEARCHRESULT>,new() where SEARCHRESULT:SearchSongResultBase,new()
    {
        public int DurationThresholdValue { get; set; } = 1000;

        public DOWNLOADER Downloader { get; } = new DOWNLOADER();
        public SEARCHER Seadrcher { get; } = new SEARCHER();
        public PARSER Parser { get; } = new PARSER();

        public override Lyrics ProvideLyric(string artist, string title, int time)
        {
            var search_result = Seadrcher.Search(artist, title).Result;

            if (Utils.DebugMode)
            {
                foreach (var r in search_result)
                {
                    Utils.Debug($"- music_id:{r.ID} artist:{r.Artist} title:{r.Title} time{r.Duration}({Math.Abs(r.Duration - time):F2})");
                }
            }

            search_result.RemoveAll((r) => Math.Abs(r.Duration - time) > DurationThresholdValue);

            string check_Str = $"{artist}{title}";

            search_result.Sort((a, b) => _GetEditDistance(a) - _GetEditDistance(b));

            if (search_result.Count == 0)
            {
                return null;
            }

            if (Utils.DebugMode)
            {
                foreach (var r in search_result)
                {
                    Utils.Debug($"+ music_id:{r.ID} artist:{r.Artist} title:{r.Title} time{r.Duration}({Math.Abs(r.Duration - time):F2})");
                }
            }

            var result = search_result.First();

            Utils.Debug($"* Picked music_id:{result.ID} artist:{result.Artist} title:{result.Title}");

            var lyric_cont = Downloader.DownloadLyric(result);

            if (string.IsNullOrWhiteSpace(lyric_cont))
            {
                return null;
            }

            return Parser.Parse(lyric_cont);

            int _GetEditDistance(SearchSongResultBase s) => Utils.EditDistance($"{s.Artist}{s.Title}", check_Str);
        }
    }
}
