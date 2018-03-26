[English Version](./README-en.MD)

简介
---
这个是Sync的插件,借助ORTDP插件和OLSP插件,获取获取当前打图歌曲并实时将对应歌词输出到文本文件中供其他用途.

使用前提
---
* [Sync](https://github.com/OsuSync/Sync)

**插件依赖**:
* [ORTDP](https://github.com/KedamaOvO/OsuRTDataProvider-Release)  
* [OsuLiveStatusPanel](https://github.com/MikiraSora/OsuLiveStatusPanel)  

使用
---
0. 如果没安装ORTDP和OsuLiveStatusPanel插件,请先安装它们.
1. 从[Release](https://github.com/OsuSync/LyricDisplayerPlugin/releases)页面下载最新插件压缩包(**注意版本要求**).
2. 将压缩包内容解压到Sync根目录即可,若有覆盖提示请全允许.
3. Enjoy.当你**打图的时候**,如果能抓到歌词,本插件会定时向文本文件输出对应歌词,打图完成后或者非打图情况则自动清除内容.

截图
---
![](https://puu.sh/zksKs/e88b63560f.png)

项目/轮子引用
---
* [ATL](https://github.com/Zeugma440/atldotnet)
* [ORTDP](https://github.com/KedamaOvO/OsuRTDataProvider-Release)
* [OsuLiveStatusPanel](https://github.com/MikiraSora/OsuLiveStatusPanel)
* [Sync](https://github.com/OsuSync/Sync)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

支持的"歌词源"(可自动寻找):
* [网易云](http://music.163.com/)(支持获取翻译歌词)
* [QQ音乐](https://y.qq.com/)(支持获取翻译歌词)
* [酷狗](http://www.kugou.com/)

config.ini中此插件配置选项列表
<br>
---
| 设置名称     | 值|默认值| 描述|
|:---------|:---------|:---------|:-------|
|LyricsSource|字符串|auto|要搜索和获取的歌词源(可选选项:auto/netease/qqmusic/kugou)|
|LyricsSentenceOutputPath|字符串|..\lyric.txt|实时输出歌词保存路径|
|DebugMode|True/False|False|调试模式|
|EnableOutputSearchResult|True/False|False|输出成功获取歌词结果,供其他用途(输出在..\lyrics_cache\\{歌词源}.txt,每一行都是可直接解析的json对象)|
|PreferTranslateLyrics|True/False|False|优先选择翻译歌词(若没有再找原版歌词)|
|BothLyrics|True/False|False|优先同时输出翻译歌词和原版歌词,一行为原版歌词,新一行为翻译歌词)|
|SearchAndDownloadTimeout|整数数值|2000|寻找和下载歌词的时限(毫秒为单位)|
