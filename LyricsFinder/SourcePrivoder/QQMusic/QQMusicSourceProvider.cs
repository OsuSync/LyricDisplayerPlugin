using System.Collections.Generic;
using System.Linq;

namespace LyricsFinder.SourcePrivoder.QQMusic
{
    [SourceProviderName("qqmusic", "DarkProjector")]
    public class QQMusicSourceProvider : SourceProviderBase<Song, QQMusicSearch, QQMusicLyricDownloader, DefaultLyricsParser>
    {
        public override Lyrics PickLyric(string artist, string title, int time, List<Song> search_result, bool request_trans_lyrics, out Song picked_result)
        {
            picked_result=null;

            var result = base.PickLyric(artist, title, time, search_result, request_trans_lyrics, out Song temp_picked_result);

            if (result!=null)
            {
                switch (result.LyricSentencs.Count)
                {
                    case 0:
                        Utils.Debug($"{picked_result?.ID}:无任何歌词在里面,rej");
                        return null;

                    case 1:
                        var first_sentence = result.LyricSentencs.First();
                        if (first_sentence.StartTime<=0&&first_sentence.Content.Contains("纯音乐")&&first_sentence.Content.Contains("没有填词"))
                        {
                            Utils.Debug($"{picked_result?.ID}:纯音乐? : "+first_sentence);
                            return null;
                        }
                        break;

                    default:
                        break;
                }
            }

            picked_result=temp_picked_result;
            return result;
        }
    }
}