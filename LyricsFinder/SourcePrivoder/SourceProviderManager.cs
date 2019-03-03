using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LyricsFinder.SourcePrivoder
{
    public static class SourceProviderManager
    {
        public static List<Type> LyricsSourceProvidersType { get; } = new List<Type>();
        private static HashSet<(SourceProviderBase obj, string name)> cache_obj = new HashSet<(SourceProviderBase obj, string name)>();

        public static SourceProviderBase GetOrCreateSourceProvier(string provider_name)
        {
            var cache = cache_obj.FirstOrDefault(x => x.name.Equals(provider_name, StringComparison.InvariantCultureIgnoreCase));

            if (cache.obj!=null)
                return cache.obj;

            var provider = LyricsSourceProvidersType.FirstOrDefault(x => x.GetCustomAttribute<SourceProviderNameAttribute>()?.Name?.Equals(provider_name, StringComparison.InvariantCultureIgnoreCase)??false);

            if (provider!=null)
            {
                var obj = provider.Assembly.CreateInstance(provider.FullName) as SourceProviderBase;
                cache_obj.Add((obj, provider_name));

                return obj;
            }

            return null;
        }
    }
}