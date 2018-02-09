using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            Init();
        }

        private void Init()
        {
            switch (((string)LyricsSource).ToLower())
            {
                case "netease":
                    lyrics_provider = new NeteaseSourceProvider();
                    break;
                default:
                    Utils.Output("未知歌词源:"+LyricsSource, ConsoleColor.Red);
                    lyrics_provider = null;
                    break;
            }

            if (lyrics_provider==null)
            {
                Utils.Output("初始化失败,请确认配置是否正确",ConsoleColor.Red);
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

        private void OnCurrentPlayTimeChanged(int time)
        {
            if (current_lyrics==null)
            {

            }
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

        #region MyRegion

        private Lyrics GetLyric()
        {
            return null;//todo
        }

        #endregion
    }
}
