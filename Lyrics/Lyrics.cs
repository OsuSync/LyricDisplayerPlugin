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

        public Lyrics() => LyricSentencs = new List<Sentence>();

        public Lyrics(IEnumerable<Sentence> sentences) => LyricSentencs = new List<Sentence>(sentences);

        public (Sentence, int) GetCurrentSentence(int time)
        {
            var index = LyricSentencs.FindLastIndex((s) => s.StartTime <= time);

            return (index < 0 ? Sentence.Empty : LyricSentencs[index], index);
        }
    }
}
