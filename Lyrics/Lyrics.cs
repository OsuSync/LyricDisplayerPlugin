using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin
{
    public class Lyrics
    {
        public static Lyrics Empty { get; private set; } = new Lyrics(new List<Sentence>());

        public List<Sentence> LyricSentencs { get; set; }

        public bool IsTranslatedLyrics { get; set; }

        /// <summary>
        /// 指要搜寻的内容的歌曲信息
        /// </summary>
        public Info RawInfo { get; set; }

        /// <summary>
        /// 指要搜寻的内容的歌曲信息
        /// </summary>
        public Info QueryInfo { get; set; }

        public Lyrics() : this(new List<Sentence>()) { }

        public Lyrics(IEnumerable<Sentence> sentences, bool is_trans_lyrics = false)
        {
            LyricSentencs = new List<Sentence>(sentences);
            IsTranslatedLyrics = is_trans_lyrics;
        }

        public (Sentence, int) GetCurrentSentence(int time)
        {
            var index = LyricSentencs.FindLastIndex((s) => s.StartTime <= time);

            return (index < 0 ? Sentence.Empty : LyricSentencs[index], index);
        }
    }
}
