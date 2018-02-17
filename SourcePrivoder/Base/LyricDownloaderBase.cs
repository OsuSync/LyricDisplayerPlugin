using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder
{
    public abstract class LyricDownloaderBase
    {
        public abstract string DownloadLyric(SearchSongResultBase song,out bool response_is_trans_lyrics,bool request_trans_lyrics=false);
    }
}
