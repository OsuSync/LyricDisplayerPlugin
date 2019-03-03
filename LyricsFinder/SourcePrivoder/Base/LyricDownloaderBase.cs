namespace LyricsFinder.SourcePrivoder
{
    public abstract class LyricDownloaderBase
    {
        public abstract string DownloadLyric(SearchSongResultBase song, bool request_trans_lyrics = false);
    }
}