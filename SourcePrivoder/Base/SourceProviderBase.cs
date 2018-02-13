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
            try
            {
                var search_result = Seadrcher.Search(artist, title);

                return PickLyric(artist, title, time, search_result);
            }
            catch (Exception e)
            {
                Utils.Output($"{GetType().Name}获取歌词失败:{e.Message}",ConsoleColor.Red);
                return null;
            }
        }

        public virtual Lyrics PickLyric(string artist, string title, int time,List<SEARCHRESULT> search_result)
        {
            DumpSearchList("-", time, search_result);

            FuckSearchFilte(artist, title, time, ref search_result);

            DumpSearchList("+", time, search_result);

            if (search_result.Count == 0)
                return null;

            var result = search_result.First();

            Utils.Debug($"* Picked music_id:{result.ID} artist:{result.Artist} title:{result.Title}");

            var lyric_cont = Downloader.DownloadLyric(result);

            if (string.IsNullOrWhiteSpace(lyric_cont))
                return null;

            return Parser.Parse(lyric_cont);
        }

        private static void DumpSearchList(string prefix,int time,List<SEARCHRESULT> search_list)
        {
            if (Utils.DebugMode)
                foreach (var r in search_list)
                    Utils.Debug($"{prefix} music_id:{r.ID} artist:{r.Artist} title:{r.Title} time{r.Duration}({Math.Abs(r.Duration - time):F2})");
        }

        public virtual void FuckSearchFilte(string artist, string title, int time,ref List<SEARCHRESULT> search_result)
        {
            //删除长度不对的
            search_result.RemoveAll((r) => Math.Abs(r.Duration - time) > DurationThresholdValue);

            string check_Str = $"{artist}{title}";

            float threhold_len = check_Str.Length * 0.3f;

            /*
            //删除看起来不匹配的(超过1/3内容不对就出局)
            search_result.RemoveAll((r) => _GetEditDistance(r) > threhold_len);
            */

            //search_result.Sort((a, b) => Math.Abs(a.Duration - time) - Math.Abs(b.Duration - time));
            search_result.Sort((a, b) => _GetEditDistance(a) - _GetEditDistance(b));

            int _GetEditDistance(SearchSongResultBase s) => Utils.EditDistance($"{s.Artist}{s.Title}", check_Str);
        }
    }
}
