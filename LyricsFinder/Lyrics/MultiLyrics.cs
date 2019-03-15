using System;
using System.Collections.Generic;
using System.Linq;

namespace LyricsFinder
{
    public class MultiLyrics : Lyrics
    {
        private readonly IEnumerable<Lyrics> lyricses;
        
        public MultiLyrics(params Lyrics[] lyricses)
        {
            this.lyricses=lyricses.OfType<Lyrics>();
        }

        public override (Sentence, int) GetCurrentSentence(int time)
        {
            var result = lyricses.Select(l => l.GetCurrentSentence(time));

            if (!result.Any())
                return (Sentence.Empty, 0);

            //如果其中一份歌词返回空的，那说明这里有停顿(?
            //比如 https://puu.sh/D0bRh/569789535e.png
            if (result.Any(x=>x.Item1.Equals(Sentence.Empty)))
                return (Sentence.Empty, result.Min(x => x.Item2));

            var ret = (result.Aggregate((z, x) => (z.Item1+x.Item1, 0)).Item1, result.Min(x => x.Item2));

            //trim newline in sentence content
            ret.Item1.Content=ret.Item1.Content.Trim('\n', '\r');

            return ret;
        }
    }
}