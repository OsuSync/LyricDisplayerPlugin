using Sync.Tools;
using Sync.Tools.ConfigGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin
{
    public class Language: I18nProvider
    {
        public static GuiLanguageElement DebugMode = "调试模式";
        public static GuiLanguageElement EnableOutputSearchResult = @"输出成功结果到文件";
        public static GuiLanguageElement PreferTranslateLyrics = "优先选择翻译歌词";
        public static GuiLanguageElement LyricsSource = "歌词源";
        public static GuiLanguageElement LyricsSentenceOutputPath = "歌词保存路径";
        public static GuiLanguageElement BothLyrics = "输出翻译&原版歌词";
        public static GuiLanguageElement SearchAndDownloadTimeout = "寻找和下载歌词的时限";
        public static GuiLanguageElement GobalTimeOffset = "全局歌词时间轴延迟";
        public static GuiLanguageElement ForceKeepTime = "限制显示歌词最长时间";
        public static GuiLanguageElement StrictMatch = "严格筛选模式";

        public static GuiLanguageElement DebugModeDescription = "调试模式,开启的话将会输出搜索过程和结果，以及实时输出歌词在控制台";
        public static GuiLanguageElement EnableOutputSearchResultDescription = @"输出成功获取歌词结果,供其他用途(输出在..\lyrics_cache\{歌词源}.txt,每一行都是可直接解析的json对象)";
        public static GuiLanguageElement PreferTranslateLyricsDescription = "若没有再找原版歌词";
        public static GuiLanguageElement BothLyricsDescription = "优先同时输出翻译歌词和原版歌词,一行为原版歌词,新一行为翻译歌词)";
        public static GuiLanguageElement SearchAndDownloadTimeoutDescription = "毫秒,寻找和下载歌词的时限";
        public static GuiLanguageElement GobalTimeOffsetDescription = "毫秒,>0为提前,<0推迟";
        public static GuiLanguageElement ForceKeepTimeDescription = "毫秒，0为不限制";
        public static GuiLanguageElement StrictMatchDescription = "开启的话将会更加严格的筛选搜索结果";
    }
}
