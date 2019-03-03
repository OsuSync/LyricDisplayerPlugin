using LyricsFinder.SourcePrivoder;
using LyricsFinder.SourcePrivoder.Netease;
using static LyricsFinder.SourcePrivoder.Netease.NeteaseSearch;

namespace LyricsFinder
{
    [SourceProviderName("netease", "DarkProjector")]
    public class NeteaseSourceProvider : SourceProviderBase<Song, NeteaseSearch, NeteaseLyricDownloader, NeteaseLyricParser>
    {
    }
}