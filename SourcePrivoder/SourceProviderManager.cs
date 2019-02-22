using Sync;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder
{
    public static class SourceProviderManager
    {
        public static List<Type> LyricsSourceProvidersType { get; } = new List<Type>();
        private static HashSet<(SourceProviderBase obj,string name)> cache_obj = new HashSet<(SourceProviderBase obj, string name)>();

        public static void ScanAvaliableLyricsSourceProvider()
        {
            try
            {
                var host = SyncHost.Instance;
                var manager = host.GetType().GetField("plugins", BindingFlags.NonPublic|BindingFlags.Instance).GetValue(host);

                var father_type = typeof(SourceProviderBase);

                var list=manager.GetType().GetField("asmList", BindingFlags.NonPublic|BindingFlags.Instance).GetValue(manager) as List<Assembly>;

                foreach (var type in list.SelectMany(l=>l.ExportedTypes))
                {
                    try
                    {
                        var name = type.Name;
                        if ((type.IsSubclassOf(father_type)||type.IsAssignableFrom(father_type))
                            &&!type.IsAbstract
                            &&type.GetCustomAttribute<SourceProviderNameAttribute>() is SourceProviderNameAttribute attr
                            &&!LyricsSourceProvidersType.Contains(type))
                        {
                            Utils.Output($"加载{attr.Author}的歌词源:{type.Name}", ConsoleColor.Green);
                            LyricsSourceProvidersType.Add(type);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception e)
            {
                Utils.Output("无法获取已加载的assembly", ConsoleColor.Red);
            }
        }

        public static SourceProviderBase GetOrCreateSourceProvier(string provider_name)
        {
            var cache = cache_obj.FirstOrDefault(x => x.name.Equals(provider_name, StringComparison.InvariantCultureIgnoreCase));

            if (cache.obj!=null)
                return cache.obj;

            var provider = LyricsSourceProvidersType.FirstOrDefault(x => x.GetCustomAttribute<SourceProviderNameAttribute>()?.Name?.Equals(provider_name, StringComparison.InvariantCultureIgnoreCase)??false);

            if (provider!=null)
            {
                var obj = provider.Assembly.CreateInstance(provider.FullName) as SourceProviderBase;
                cache_obj.Add((obj,provider_name));

                return obj;
            }

            return null;
        }
    }
}
