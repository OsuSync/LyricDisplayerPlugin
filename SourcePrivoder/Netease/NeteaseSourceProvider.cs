using LyricDisplayerPlugin.SourcePrivoder;
using LyricDisplayerPlugin.SourcePrivoder.Netease;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LyricDisplayerPlugin.SourcePrivoder.Netease.NeteaseSearch;

namespace LyricDisplayerPlugin
{
    [SourceProviderName("netease", "DarkProjector")]
    public class NeteaseSourceProvider:SourceProviderBase<Song,NeteaseSearch, NeteaseLyricDownloader,NeteaseLyricParser>
    {

    }
}
