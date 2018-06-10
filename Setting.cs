using Sync.Tools.ConfigGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin
{
    public static class Setting
    {
        [Bool(Description = "调试模式")]
        public static bool DebugMode { get; set; } = false;

        [Bool(Description = @"输出成功获取歌词结果,供其他用途(输出在..\lyrics_cache\{歌词源}.txt,每一行都是可直接解析的json对象)")]
        public static bool EnableOutputSearchResult { get; set; } = false;

        [Bool(Description = "优先选择翻译歌词(若没有再找原版歌词)")]
        public static bool PreferTranslateLyrics { get; set; } = false;

        [List(AllowMultiSelect =false,Description = "要搜索和获取的歌词源(可选选项:auto/netease/qqmusic/kugou)",ValueList =new[] 
        {"auto","netease","qqmusic","kugou"}
        ,IgnoreCase =true)]
        public static string LyricsSource { get; set; } = "auto";

        [Path(Description = "实时输出歌词保存路径",IsDirectory =true,RequireExist =true)]
        public static string LyricsSentenceOutputPath { get; set; } = @"..\lyric.txt";
        
        //内部保留
        public static bool IsUsedByPlugin { get;internal set; } = false;

        [Bool(Description = "优先同时输出翻译歌词和原版歌词,一行为原版歌词,新一行为翻译歌词)")]
        public static bool BothLyrics { get; internal set; } = true;

        [Integer(Description = "寻找和下载歌词的时限(毫秒)")]
        public static int SearchAndDownloadTimeout { get; internal set; } = 2000;

        [Integer(Description = "全局歌词时间轴延迟（毫秒）")]
        public static int GobalTimeOffset { get; internal set; } = 0;

        [Integer(Description = "限制显示歌词最长时间（毫秒，0为不限制）")]
        public static uint ForceKeepTime { get; internal set; } = 0;

        [Bool(Description = "严格筛选歌词模式")]
        public static bool StrictMatch { get; internal set; } = true;
    }
}
