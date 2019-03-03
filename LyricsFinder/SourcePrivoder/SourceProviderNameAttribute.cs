using System;

namespace LyricsFinder.SourcePrivoder
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SourceProviderNameAttribute : Attribute
    {
        public SourceProviderNameAttribute(string name, string author)
        {
            Name=name;
            Author=author;
        }

        public string Name { get; }
        public string Author { get; }
    }
}