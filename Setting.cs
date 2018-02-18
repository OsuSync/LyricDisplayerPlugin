using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin
{
    public static class Setting
    {
        public static bool DebugMode { get; set; } = false;

        public static bool EnableOutputSearchResult { get; set; } = false;

        public static bool PreferTranslateLyrics { get; set; } = false;

        public static string LyricsSource { get; set; } = "auto";

        public static string LyricsSentenceOutputPath { get; set; } = @"..\lyric.txt";

        public static bool IsUsedByPlugin { get;internal set; } = false;
    }
}
