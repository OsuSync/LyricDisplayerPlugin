using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder
{
    public abstract class SongSearchBase
    {
        public abstract Task<List<SearchSongResultBase>> Search(params string[] param_arr);
    }
}
