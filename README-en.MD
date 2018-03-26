Decription
---
This is a osu!sync plugin which able to output real-time lyrics to file when you are playing,it's depend on others osu!sync plugins named "ORTDP" and "OsuLiveStatusPanel".

Requirement 
---
* [Sync](https://github.com/OsuSync/Sync)
<br>
**Dependence Plugins**:
<br>
* [ORTDP](https://github.com/KedamaOvO/OsuRTDataProvider-Release)
* [OsuLiveStatusPanel](https://github.com/MikiraSora/OsuLiveStatusPanel)

Install&Usage
---
0. Please install plugin ORTDP and OsuLiveStatusPanel before you start installing this plugin. 
1. Download latest release zip file from [Release](https://github.com/OsuSync/LyricDisplayerPlugin/releases)(**Attention to version requirements**).
2. Extract zip file and cover them to Sync folder.
3. Enjoy.**When you are playing**,if the plugin got lyrics and it will output lyrics content to file following playing time. It's automatic hide when you are not playing.

Screenshots
---
![](https://puu.sh/zksKs/e88b63560f.png)

Project/Library Reference
---
* [ATL](https://github.com/Zeugma440/atldotnet)
* [ORTDP](https://github.com/KedamaOvO/OsuRTDataProvider-Release)
* [OsuLiveStatusPanel](https://github.com/MikiraSora/OsuLiveStatusPanel)
* [Sync](https://github.com/OsuSync/Sync)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

Supported Lyrics Source:
---
* [NeteaseMusic](http://music.163.com/)(Support get translated chinese lyrics)
* [QQMusic](https://y.qq.com/)(Support get translated chinese lyrics)
* [Kugou](http://www.kugou.com/)

config.ini configuration list
<br>
---
| 设置名称     | 值|默认值| 描述|
|:---------|:---------|:---------|:-------|
|LyricsSource|string|auto|Select lyrics source(selectable list:auto/netease/qqmusic/kugou)|
|LyricsSentenceOutputPath|string|..\lyric.txt|Output lyrics file path|
|DebugMode|True/False|False|Debug Mode|
|EnableOutputSearchResult|True/False|False|output records which get lyrics successfully(输出在..\lyrics_cache\\{Lyrics Source}.txt,Each line is json object you can parse it directly)|
|PreferTranslateLyrics|True/False|False|prioritize to select translated lyrics(if not found and try to get raw lyrics)|
|BothLyrics|True/False|False|Prioritize to select both translated/raw lyrics(Raw lyrics and translated lyrics are difference line)|
|SearchAndDownloadTimeout|Integer Value|2000|The timeout value(ms) of searching&downloading lyrics|
