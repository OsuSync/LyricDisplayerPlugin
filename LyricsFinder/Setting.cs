namespace LyricsFinder
{
    public static class Setting
    {
        public static bool DebugMode { get; set; } = false;
        public static bool EnableOutputSearchResult { get; set; } = false;
        public static bool PreferTranslateLyrics { get; set; } = false;
        public static string LyricsSource { get; set; } = "auto";
        public static string LyricsSentenceOutputPath { get; set; } = @"..\lyric.txt";
        
        public static bool BothLyrics { get; set; } = true;
        public static int SearchAndDownloadTimeout { get; set; } = 2000;
        public static int GobalTimeOffset { get; set; } = 0;
        public static uint ForceKeepTime { get; set; } = 0;
        public static bool StrictMatch { get; set; } = true;

        /// <summary>
        /// 下一版本会钦定用动态形式的(如果那个没问题的话)
        /// </summary>
        public static bool UseStaticLyricsCombine { get; set; } = false;
    }
}