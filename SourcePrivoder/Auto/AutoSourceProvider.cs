using LyricDisplayerPlugin.SourcePrivoder.Kugou;
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
            new QQMusicSourceProvider(),
            new KugouSourceProvider()
        };

        public override Lyrics ProvideLyric(string artist, string title, int time)
        {
            Task<Lyrics>[] tasks = new Task<Lyrics>[search_engines.Length];

            System.Threading.CancellationTokenSource token = new System.Threading.CancellationTokenSource();
            
            for (int i = 0; i < search_engines.Length; i++)
                tasks[i] = Task.Factory.StartNew<Lyrics>((index) => search_engines[(int)index].ProvideLyric(artist, title, time),i,token.Token);

            Lyrics lyrics = null;

            for (int i = 0; i < search_engines.Length; i++)
            {
                lyrics = tasks[i].Result;

                //如果是刚好是要相同版本的歌词那可以直接返回了,否则就等一下其他源是不是还能拿到合适的版本
                if (lyrics?.IsTranslatedLyrics==Utils.PreferTranslateLyrics)
                {
                    token.Cancel();
                    Utils.Debug($"Quick select lyric from {search_engines[i].GetType().Name}");
                    break;
                }
            }

            return lyrics;
        }
    }
}
