using System.Collections.Generic;

namespace LyricsFinder.SourcePrivoder
{
    public abstract class SongSearchBase<T> where T : SearchSongResultBase, new()
    {
        public abstract List<T> Search(params string[] param_arr);
    }
}