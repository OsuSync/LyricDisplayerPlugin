using LyricDisplayerPlugin.SourcePrivoder.QQMusic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder.Auto
{
    public class AutoSourceProvider : SourceProviderBase
    {
        public SourceProviderBase[] search_engines = new SourceProviderBase[]{
            new NeteaseSourceProvider(),
            new QQMusicSourceProvider()
        };

        public override Lyrics ProvideLyric(string artist, string title, int time)
        {
            foreach (var provider in search_engines)
            {
                Lyrics result = provider.ProvideLyric(artist, title, time);

                if (result != null)
                {
                    Utils.Debug($"Auto select lyric from {provider.ToString()}");
                    return result;
                }
            }

            return null;
        }
    }
}
