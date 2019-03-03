using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LyricsFinder.SourcePrivoder.Netease
{
    public class NeteaseLyricParser : LyricParserBase
    {
        private static Regex lyric_regex = new Regex(@"\[(\d{2}\d*)\:(\d{2})\.(\d*)?\](.*?)(\r)?\n");

        public override Lyrics Parse(string content)
        {
            List<Sentence> sentence_list = new List<Sentence>();

            var match = lyric_regex.Match(content);

            while (match.Success)
            {
                int min = int.Parse(match.Groups[1].Value.ToString()), sec = int.Parse(match.Groups[2].Value.ToString()), msec = int.Parse(match.Groups[3].Value.ToString());

                string cont = match.Groups[4].Value.ToString().Trim();

                int time = min*60*1000+sec*1000+msec;

                sentence_list.Add(new Sentence(cont, time));

                match=match.NextMatch();
            }

            sentence_list.Sort();

            return new Lyrics(sentence_list);
        }
    }
}