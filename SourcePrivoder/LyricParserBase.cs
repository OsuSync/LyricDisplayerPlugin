using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin.SourcePrivoder
{
    public abstract class LyricParserBase
    {
        public abstract Lyrics Parse(string content);
    }
}
