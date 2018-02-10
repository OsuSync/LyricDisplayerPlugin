using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder.QQMusic
{
    public static class QQMusicLyricParser
    {
        private static Regex lyric_regex = new Regex(@"\[(\d{2}\d*)\:(\d{2})\.(\d*)?\](.*)");

        public static int ParseTime(string raw_str)
        {
            return 0;
        }

        public static Lyrics Parse(string content)
        {
            List<Sentence> sentence_list = new List<Sentence>();

            StreamReader reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(content)));

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                var match = lyric_regex.Match(line);
                if (!match.Success)
                {
                    continue;
                }
                int min = int.Parse(match.Groups[1].Value.ToString()), sec = int.Parse(match.Groups[2].Value.ToString()), msec = int.Parse(match.Groups[3].Value.ToString());

                string cont = match.Groups[4].Value.ToString().Trim();

                int time = min * 60 * 1000 + sec * 1000 + msec;

                Console.WriteLine($"{time} \t {cont}");

                sentence_list.Add(new Sentence(cont, time));
            }

            reader.Close();

            sentence_list.Sort();

            return new Lyrics(sentence_list);
        }
    }
}
