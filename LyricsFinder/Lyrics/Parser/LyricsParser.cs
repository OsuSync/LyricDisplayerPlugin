using LyricsFinder.SourcePrivoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LyricsFinder
{
    public class DefaultLyricsParser: LyricParserBase
    {
        const string TIMELINE_REGEX = @"\[(\d+\:)+\d+(\.\d+)?\]";
        private static Regex timeline_parse_regex = new Regex(@"\[(\d+\d*)\:(\d+)\.(\d*)?\]");
        private static Regex timeline_regex = new Regex(TIMELINE_REGEX);
        private static Regex lyric_regex = new Regex($"(({TIMELINE_REGEX})+)(.*)");

        public static IEnumerable<Sentence> ParseLyricsContent(string lyrics_content)
        {
            using (var reader=new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(lyrics_content))))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var match = lyric_regex.Match(line);

                    if (!match.Success)
                        continue;

                    var content = match.Groups[5].Value;

                    var timeline_matches = timeline_regex.Matches(match.Groups[1].Value);

                    foreach (Match m in timeline_matches)
                    {
                        var zz = timeline_parse_regex.Match(m.Groups[0].Value);
                        int min = int.Parse(zz.Groups[1].Value.ToString()), sec = int.Parse(zz.Groups[2].Value.ToString()), msec = int.Parse(zz.Groups[3].Value.ToString());

                        int time = min*60*1000+sec*1000+msec;

                        Sentence sentence =new Sentence(content, time);

                        yield return sentence;
                    }
                }
            }
        }

        public override Lyrics Parse(string content)
        {
            try
            {
                var sentences = ParseLyricsContent(content);

                return new Lyrics(sentences);
            }
            catch (Exception e)
            {
                Utils.Output($"Parse lyrics content failed!"+e.Message, ConsoleColor.Red);
                return null;
            }
        }
    }
}
