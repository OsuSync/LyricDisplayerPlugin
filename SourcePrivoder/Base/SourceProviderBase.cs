using LyricDisplayerPlugin.SourcePrivoder;
using LyricDisplayerPlugin.SourcePrivoder.QQMusic;
using System;
using System.Collections.Generic;
using System.IO;
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

                var lyrics= PickLyric(artist, title, time, search_result,out SEARCHRESULT picked_result);

                //过滤没有实质歌词内容的玩意,比如没有时间轴的歌词文本
                if (lyrics?.LyricSentencs?.Count==0)
                    return null;

                if (lyrics!=null&&Utils.EnableOutputSearchResult)
                {
                    //output lyrics search result
                    var content_obj = new { DateTime=DateTime.Now,picked_result.ID,picked_result.Artist,picked_result.Title,picked_result.Duration,Raw_Title=title,Raw_Artist=artist,Raw_Duration=time };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(content_obj,Newtonsoft.Json.Formatting.None);
                    if (!Directory.Exists(@"..\lyric_cache"))
                        Directory.CreateDirectory(@"..\lyric_cache");
                    string file_path = $@"..\lyric_cache\{this.GetType().Name}.txt";
                    File.AppendAllText(file_path, json+Environment.NewLine, Encoding.UTF8);
                }

                return lyrics;
            }
            catch (Exception e)
            {
                Utils.Output($"{GetType().Name}获取歌词失败:{e.Message}",ConsoleColor.Red);
                return null;
            }
        }

        public virtual Lyrics PickLyric(string artist, string title, int time,List<SEARCHRESULT> search_result,out SEARCHRESULT picked_result)
        {
            picked_result = null;

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

            picked_result = result;

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
