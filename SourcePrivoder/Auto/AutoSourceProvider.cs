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
            Task<Lyrics>[] tasks = new Task<Lyrics>[search_engines.Length];

            System.Threading.CancellationTokenSource token = new System.Threading.CancellationTokenSource();
            
            for (int i = 0; i < search_engines.Length; i++)
                tasks[i] = Task.Factory.StartNew<Lyrics>((index) => search_engines[(int)index].ProvideLyric(artist, title, time),i,token.Token);

            for (int i = 0; i < search_engines.Length; i++)
            {
                var result = tasks[i].Result;
                if (result!=null)
                {
                    token.Cancel();
                    Utils.Debug($"Auto select lyric from {search_engines[i].GetType().Name}");
                    return result;
                }
            }

            return null;
        }
    }
}
