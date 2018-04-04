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

        public static bool BothLyrics { get; internal set; } = true;

        public static int SearchAndDownloadTimeout { get; internal set; } = 2000;

        public static int GobalTimeOffset { get; internal set; } = 0;

        public static uint ForceKeepTime { get; internal set; } = 0;
    }
}
