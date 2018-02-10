using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATL;
using OsuLiveStatusPanel;
using OsuRTDataProvider;
using Sync;
using Sync.Plugins;
using Sync.Tools;

namespace LyricDisplayerPlugin
{

    [SyncRequirePlugin(typeof(OsuLiveStatusPanelPlugin),typeof(OsuRTDataProviderPlugin))]
    public class LyricDisplayerPlugins : Plugin,IConfigurable
    {
        public ConfigurationElement LyricsSource { get; set; } = "Netease";

        public ConfigurationElement LyricsSentenceOutputPath { get; set; } = @"..\lyric.txt";

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
            switch (((string)LyricsSource).ToLower())
            {
                case "netease":
                    lyrics_provider = new NeteaseSourceProvider();
                    break;
                case "qqmusic":
                    lyrics_provider = new SourcePrivoder.QQMusic.QQMusicSourceProvider();
                    break;
                default:
                    Utils.Output("未知歌词源:"+LyricsSource, ConsoleColor.Red);
                    lyrics_provider = null;
                    break;
            }
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
                Utils.Debug($"[cur:{time} now:{sentence_query.Item1.StartTime}]{sentence_query.Item1.Content}");
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
        }

        private void OutputLyricSentence(Sentence sentence)
        {
            File.WriteAllText(LyricsSentenceOutputPath, sentence.Content);
        }

        #region Get/Save Lyrics

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
            if (lyrics==Lyrics.Empty)
            {
                return;
            }

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(lyrics, Newtonsoft.Json.Formatting.Indented);

            if (Directory.Exists(@"..\lyric_cache"))
            {
                Directory.CreateDirectory(@"..\lyric_cache");
            }

            File.WriteAllText($@"..\lyric_cache\{artist}-{title}({time}).txt",data);
        }

        private bool TryGetLyricFromCacheFile(string title,string artist,int time,out Lyrics lyrics)
        {
            //todo
            lyrics = Lyrics.Empty;

            var file_path = $@"..\lyric_cache\{artist}-{title}({time}).txt";

            if (!File.Exists(file_path))
            {
                return false;
            }

            try
            {
                var data = File.ReadAllText(file_path);
                lyrics = Newtonsoft.Json.JsonConvert.DeserializeObject<Lyrics>(data);
            }
            catch (Exception e)
            {
                Utils.Output("读取歌词缓存文件失败,原因" + e.Message,ConsoleColor.Yellow);
                throw;
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
            string artist = olsp_plugin.GetData("artist_avaliable");
            string title = olsp_plugin.GetData("title_avaliable");
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
            OutputCache(title, artist, time, lyrics);

            return l;
        }

        #endregion
    }
}
