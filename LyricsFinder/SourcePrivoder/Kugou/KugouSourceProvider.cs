namespace LyricsFinder.SourcePrivoder.Kugou
{
    [SourceProviderName("kugou", "DarkProjector")]
    public class KugouSourceProvider : SourceProviderBase<KugouSearchResultSong, KugouSearcher, KugouLyricDownloader, Netease.NeteaseLyricParser>
    {
    }
}