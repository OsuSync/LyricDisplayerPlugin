using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin
{
    public static class Utils
    {
        public static void Output(string message, ConsoleColor color, bool new_line = true, bool time = true)
        {
            IO.CurrentIO.WriteColor("[LyricDisplayer]" + message, color, new_line, time);
        }
    }
}
