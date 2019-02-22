using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SourceProviderNameAttribute : Attribute
    {
        public SourceProviderNameAttribute(string name,string author)
        {
            Name=name;
            Author=author;
        }

        public string Name { get; }
        public string Author { get; }
    }
}
