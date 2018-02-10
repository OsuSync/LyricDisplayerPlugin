using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder
{
    public abstract class SearchSongResultBase
    {
        public abstract string Title { get;}
        public abstract string Artist { get;  }
        public abstract int Duration { get;  }
        public abstract String ID { get; }
    }
}
