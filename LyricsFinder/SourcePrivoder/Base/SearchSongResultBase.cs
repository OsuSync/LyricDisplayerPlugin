using System;

namespace LyricsFinder.SourcePrivoder
{
    public abstract class SearchSongResultBase
    {
        public abstract string Title { get; }
        public abstract string Artist { get; }
        public abstract int Duration { get; }
        public abstract String ID { get; }
    }
}