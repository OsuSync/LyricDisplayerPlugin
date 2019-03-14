using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LyricsFinder.SourcePrivoder.Auto
{
    [SourceProviderName("auto", "DarkProjector")]
    public class AutoSourceProvider : SourceProviderBase
    {
        public SourceProviderBase[] search_engines;

        private Dictionary<string, SourceProviderBase> cache_provider =new Dictionary<string, SourceProviderBase>();

        public AutoSourceProvider()
        {
            search_engines=SourceProviderManager.LyricsSourceProvidersType
                .Select(x => x.GetCustomAttribute<SourceProviderNameAttribute>())
                .OfType<SourceProviderNameAttribute>()
                .Where(x => !x.Name.Equals("auto", StringComparison.InvariantCultureIgnoreCase))
                .Select(x => SourceProviderManager.GetOrCreateSourceProvier(x.Name))
                .ToArray();
        }

        public override Lyrics ProvideLyric(string artist, string title, int time, bool request_trans_lyrics)
        {
            var id = artist+title+time;

            //保证同一谱面获取的翻译歌词和原歌词都是同一个歌词源，这样歌词合并的时候会好很多
            if (cache_provider.TryGetValue(id,out var provider))
                return GetLyricFromExcplictSource(provider, artist, title, time, request_trans_lyrics);

            var lyrics = GetLyricFromAnySource(artist, title, time, request_trans_lyrics, out provider);

            if (lyrics!=null)
                cache_provider[id]=provider;

            return lyrics;
        }

        public Lyrics GetLyricFromAnySource(string artist, string title, int time, bool request_trans_lyrics,out SourceProviderBase provider)
        {
            var cancel_source = new System.Threading.CancellationTokenSource();

            var tasks = search_engines.Select(l => Task.Run(() => (l.ProvideLyric(artist, title, time, request_trans_lyrics), l), cancel_source.Token));

            provider=null;

            foreach (var task in tasks)
            {
                var result = task.Result;
                var lyrics = result.Item1;

                if (lyrics==null)
                    continue;

                try
                {
                    cancel_source.Cancel();
                }
                catch { }

                provider=result.l;

                Utils.Debug($"Quick select lyric from {result.l.GetType().Name}");

                return lyrics;
            }

            return null;
        }

        public Lyrics GetLyricFromExcplictSource(SourceProviderBase provider, string artist, string title, int time, bool request_trans_lyrics)
        {
            var lyrics = provider.ProvideLyric(artist, title, time, request_trans_lyrics);

            return lyrics;
        }
    }
}