using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATL;
using LyricDisplayerPlugin.SourcePrivoder.Auto;
using LyricDisplayerPlugin.SourcePrivoder.Kugou;
using OsuLiveStatusPanel;
using OsuRTDataProvider;
using Sync;
using Sync.Plugins;
using Sync.Tools;

namespace LyricDisplayerPlugin
{
    [SyncRequirePlugin(typeof(OsuLiveStatusPanelPlugin), typeof(OsuRTDataProviderPlugin))]
    public class LyricDisplayerPlugins : Plugin,IConfigurable
    {
        public ConfigurationElement LyricsSource { get; set; } = "auto";

        public ConfigurationElement EnableOutputSearchResult { get; set; } = "False";

        public ConfigurationElement LyricsSentenceOutputPath { get; set; } = @"..\lyric.txt";

        public ConfigurationElement DebugMode { get; set; } = "False";

        public ConfigurationElement PreferTranslateLyrics { get; set; } = "False";

        OsuLiveStatusPanelPlugin olsp_plugin;

        private PluginConfigurationManager config_manager;

        private string current_osu_file_path;

        private SourceProviderBase lyrics_provider;

        private Lyrics current_lyrics;

        public LyricDisplayerPlugins() : base("LyricDisplayerPlugin", "DarkProjector")
        {

        }

        #region Implements
        
        public void onConfigurationLoad()
        {
            
        }

        public void onConfigurationSave()
        {
            
        }

        public void onConfigurationReload()
        {
            Init();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(FirstInit);
        }

        #endregion

        private void FirstInit(PluginEvents.LoadCompleteEvent evt)
        {
            config_manager = new PluginConfigurationManager(this);
            config_manager.AddItem(this);

            OsuLiveStatusPanelPlugin olsp_plugin = (from plugin in evt.Host.EnumPluings() where plugin is OsuLiveStatusPanelPlugin select plugin).First() as OsuLiveStatusPanelPlugin;
            OsuRTDataProviderPlugin ortdp_plugin = (from plugin in evt.Host.EnumPluings() where plugin is OsuRTDataProviderPlugin select plugin).First() as OsuRTDataProviderPlugin;

            if (olsp_plugin==null||ortdp_plugin==null)
            {
                Utils.Output("未找到OLSP或者ortdp插件,初始化失败", ConsoleColor.Red);
                return;
            }

            this.olsp_plugin = olsp_plugin;

            EventBus.BindEvent<OutputInfomationEvent>(OnOLSPOutputInfomationEvent);

            ortdp_plugin.ListenerManager.OnPlayingTimeChanged += OnCurrentPlayTimeChanged;
            ortdp_plugin.ListenerManager.OnBeatmapChanged += map => current_osu_file_path = map.FilenameFull;
            ortdp_plugin.ListenerManager.OnStatusChanged += (old, now) => {
                if (now==OsuRTDataProvider.Listen.OsuListenerManager.OsuStatus.Rank)
                {
                    OnClean();
                }
            };

            Init();
            
            if (lyrics_provider == null)
            {
                Utils.Output("初始化失败,请确认配置是否正确", ConsoleColor.Red);
            }
            else
            {
                Utils.Output("初始化成功", ConsoleColor.Green);
            }
        }

        private void Init()
        {
            Utils.DebugMode = bool.Parse(DebugMode);
            Utils.EnableOutputSearchResult = bool.Parse(EnableOutputSearchResult);
            Utils.PreferTranslateLyrics = bool.Parse(PreferTranslateLyrics);

            if (Utils.PreferTranslateLyrics)
                Utils.Output("优先选择翻译歌词",ConsoleColor.Green);

            Utils.Debug("调试模式已开启");

            switch (((string)LyricsSource).ToLower())
            {
                case "netease":
                    lyrics_provider = new NeteaseSourceProvider();
                    break;
                case "qqmusic":
                    lyrics_provider = new SourcePrivoder.QQMusic.QQMusicSourceProvider();
                    break;
                case "kugou":
                    lyrics_provider = new KugouSourceProvider();
                    break;
                case "auto":
                    lyrics_provider = new AutoSourceProvider();
                    break;
                default:
                    Utils.Output("未知歌词源:"+LyricsSource, ConsoleColor.Red);
                    lyrics_provider = null;
                    return;
            }

            Utils.Output($"已选择歌词源:({LyricsSource}){lyrics_provider.GetType().Name}", ConsoleColor.Green);
        }

        private void OnOLSPOutputInfomationEvent(OutputInfomationEvent evt)
        {
            if (evt.CurrentOutputType==OutputType.Listen)
            {
                OnClean();
            }
            else
            {
                OnPlay();
            }
        }

        int prev_index = -2;

        private void OnCurrentPlayTimeChanged(int time)
        {
            if (current_lyrics==null)
            {
                return;
            }

            var sentence_query = current_lyrics.GetCurrentSentence(time);

            if (prev_index!=sentence_query.Item2)
            {
                //Utils.Debug($"[cur:{time} now:{sentence_query.Item1.StartTime}]{sentence_query.Item1.Content}");
                prev_index = sentence_query.Item2;
            }

            OutputLyricSentence(sentence_query.Item1);
        }

        private void OnPlay()
        {
            current_lyrics = GetLyric();
        }

        private void OnClean()
        {
            current_lyrics = null;
            OutputLyricSentence(Sentence.Empty);
        }

        private void OutputLyricSentence(Sentence sentence)
        {
            File.WriteAllText(LyricsSentenceOutputPath, sentence.Content);
        }

        #region Get/Save Lyrics

        private static string GetPath(string title,string artist,int time)
        {
            return (artist + title).GetHashCode().ToString() + time.ToString()+".json";
        }

        private string GetAudioFilePath(string osu_file_path)
        {
            var lines = File.ReadAllLines(osu_file_path);
            foreach (var line in lines)
            {
                if (line.StartsWith("AudioFilename:"))
                {
                    return line.Replace("AudioFilename:", string.Empty).Trim();
                }

                if (line.Contains("[Editor]"))
                {
                    return null;
                }
            }

            return null;
        }

        private int GetDurationTime(string osu_file_path)
        {
            string parent = Directory.GetParent(osu_file_path).FullName;

            string audio_path =Path.Combine(parent,GetAudioFilePath(osu_file_path));

            if (audio_path==null)
            {
                return -1;
            }

            var track = new Track(audio_path);

            Utils.Debug("duration:" + track.Duration);

            return track.Duration * 1000;//convert to ms
        }

        private void OutputCache(string title, string artist, int time,Lyrics lyrics)
        {
            if (lyrics==null||lyrics==Lyrics.Empty)
            {
                return;
            }

            if (!Directory.Exists(@"..\lyric_cache"))
            {
                Directory.CreateDirectory(@"..\lyric_cache\");
            }

            string path = @"..\lyric_cache\"+GetPath(title,artist,time);

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(lyrics);
            try
            {
                File.WriteAllText(path, data, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Utils.Output("无法写入缓存文件,原因:"+e.Message, ConsoleColor.Red);
            }

            Utils.Debug("缓存文件到" + path);
        }

        private bool TryGetLyricFromCacheFile(string title,string artist,int time,out Lyrics lyrics)
        {
            //todo
            lyrics = Lyrics.Empty;

            var file_path = @"..\lyric_cache\" + GetPath(title, artist, time);

            if (!File.Exists(file_path))
            {
                return false;
            }

            try
            {
                var data = File.ReadAllText(file_path,Encoding.UTF8).Replace(@"\r\n",string.Empty);
                lyrics = Newtonsoft.Json.JsonConvert.DeserializeObject<Lyrics>(data);
                Utils.Debug("读取缓存文件成功"+file_path);
            }
            catch (Exception e)
            {
                Utils.Output("读取歌词缓存文件失败,原因" + e.Message,ConsoleColor.Yellow);

                if (!Directory.Exists(@"..\lyric_cache\failed\"))
                    Directory.CreateDirectory(@"..\lyric_cache\failed\");

                string failed_save_path = $@"..\lyric_cache\failed\{artist}-{title}({time}).json";

                if (File.Exists(failed_save_path))
                    File.Delete(failed_save_path);

                File.Move(file_path,failed_save_path);
                return false;
            }

            return true;
        }

        private Lyrics GetLyric()
        {
            if (string.IsNullOrWhiteSpace(current_osu_file_path))
            {
                return null;
            }

            //获取基本数据
            string artist = olsp_plugin.GetData("artist_avaliable").ToString();
            string title = olsp_plugin.GetData("title_avaliable").ToString();
            int time = GetDurationTime(current_osu_file_path);

            Utils.Debug($"artist:{artist} title:{title} time:{time}");

            if (time<0)
            {
                return null;
            }

            //尝试从缓存文件中拿出歌词
            if (TryGetLyricFromCacheFile(title,artist,time,out Lyrics lyrics))
            {
                return lyrics;
            }

            var l = lyrics_provider?.ProvideLyric(artist, title, time);

            //缓存
            OutputCache(title, artist, time, l);

            return l;
        }

        #endregion
    }
}
