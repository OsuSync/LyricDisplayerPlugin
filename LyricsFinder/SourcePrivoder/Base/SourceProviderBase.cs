using LyricsFinder.SourcePrivoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LyricsFinder
{
    public abstract class SourceProviderBase
    {
        public abstract Lyrics ProvideLyric(string artist, string title, int time, bool request_trans_lyrics);
    }

    public abstract class SourceProviderBase<SEARCHRESULT, SEARCHER, DOWNLOADER, PARSER> : SourceProviderBase where DOWNLOADER : LyricDownloaderBase, new() where PARSER : LyricParserBase, new() where SEARCHER : SongSearchBase<SEARCHRESULT>, new() where SEARCHRESULT : SearchSongResultBase, new()
    {
        public int DurationThresholdValue { get; set; } = 1000;

        public DOWNLOADER Downloader { get; } = new DOWNLOADER();
        public SEARCHER Seadrcher { get; } = new SEARCHER();
        public PARSER Parser { get; } = new PARSER();

        public override Lyrics ProvideLyric(string artist, string title, int time, bool request_trans_lyrics)
        {
            try
            {
                var search_result = Seadrcher.Search(artist, title);

                var lyrics = PickLyric(artist, title, time, search_result, request_trans_lyrics, out SEARCHRESULT picked_result);

                if (lyrics!=null&&Setting.EnableOutputSearchResult)
                {
                    //output lyrics search result
                    var content_obj = new { DateTime = DateTime.Now, picked_result.ID, picked_result.Artist, picked_result.Title, picked_result.Duration, Raw_Title = title, Raw_Artist = artist, Raw_Duration = time };
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(content_obj, Newtonsoft.Json.Formatting.None);
                    if (!Directory.Exists(@"..\lyric_cache"))
                        Directory.CreateDirectory(@"..\lyric_cache");
                    string file_path = $@"..\lyric_cache\{this.GetType().Name}.txt";
                    File.AppendAllText(file_path, json+Environment.NewLine, Encoding.UTF8);

                    Utils.Output($"-> 从{this.GetType().Name}获取到 {(lyrics.IsTranslatedLyrics ? "翻译歌词" : "原歌词")}", ConsoleColor.Green);
                }

                return lyrics;
            }
            catch (Exception e)
            {
                Utils.Output($"{GetType().Name}获取歌词失败:{e.Message}", ConsoleColor.Red);
                return null;
            }
        }

        public virtual Lyrics PickLyric(string artist, string title, int time, List<SEARCHRESULT> search_result, bool request_trans_lyrics, out SEARCHRESULT picked_result)
        {
            picked_result=null;

            DumpSearchList("-", time, search_result);

            FuckSearchFilte(artist, title, time, ref search_result);

            DumpSearchList("+", time, search_result);

            if (search_result.Count==0)
                return null;

            Lyrics lyric_cont = null;
            SEARCHRESULT cur_result = null;

            foreach (var result in search_result)
            {
                var content = Downloader.DownloadLyric(result, request_trans_lyrics);
                cur_result=result;

                if (string.IsNullOrWhiteSpace(content))
                    continue;

                lyric_cont=Parser.Parse(content);

                //过滤没有实质歌词内容的玩意,比如没有时间轴的歌词文本
                if (lyric_cont?.LyricSentencs?.Count==0)
                    continue;

                Utils.Debug($"* Picked music_id:{result.ID} artist:{result.Artist} title:{result.Title}");
                break;
            }

            if (lyric_cont==null)
                return null;

            picked_result=cur_result;

            WrapInfo(lyric_cont);

            return lyric_cont;

            #region Wrap Methods

            //封装信息
            void WrapInfo(Lyrics l)
            {
                if (l==null)
                    return;

                Info raw_info = new Info()
                {
                    Artist=artist,
                    Title=title
                }, query_info = new Info()
                {
                    Artist=cur_result.Artist,
                    Title=cur_result.Title,
                    ID=cur_result.ID
                };

                l.RawInfo=raw_info;
                l.QueryInfo=query_info;
                l.IsTranslatedLyrics=request_trans_lyrics;
            }

            #endregion Wrap Methods
        }

        private static void DumpSearchList(string prefix, int time, List<SEARCHRESULT> search_list)
        {
            if (Setting.DebugMode)
                foreach (var r in search_list)
                    Utils.Debug($"{prefix} music_id:{r.ID} artist:{r.Artist} title:{r.Title} time{r.Duration}({Math.Abs(r.Duration-time):F2})");
        }

        public virtual void FuckSearchFilte(string artist, string title, int time, ref List<SEARCHRESULT> search_result)
        {
            //删除长度不对的
            search_result.RemoveAll((r) => Math.Abs(r.Duration-time)>DurationThresholdValue);

            string check_Str = $"{title.Trim()}";

            if (Setting.StrictMatch)
            {
                //删除标题看起来不匹配的(超过1/3内容不对就出局)，当然开头相同除外
                float threhold_length = check_Str.Length*(1.0f/3);
                search_result.RemoveAll((r) =>
                {
                    //XXXX和XXXXX(Full version)这种情况可以跳过
                    if (r.Title.Trim().StartsWith(check_Str))
                        return false;//不用删除，通过

                    var distance = _GetEditDistance(r);
                    return distance>threhold_length;
                }
                );
            }

            //search_result.Sort((a, b) => Math.Abs(a.Duration - time) - Math.Abs(b.Duration - time));
            search_result.Sort((a, b) => _GetEditDistance(a)-_GetEditDistance(b));

            int _GetEditDistance(SearchSongResultBase s) => Utils.EditDistance($"{s.Title}", check_Str);
        }
    }
}