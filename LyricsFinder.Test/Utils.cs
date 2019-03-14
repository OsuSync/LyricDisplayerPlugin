using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricsFinder.Test
{
    public  static class Utils
    {
        public static string ReadTestResource(string resource_relative_path)
            => File.ReadAllText(Path.Combine("TestResource/", resource_relative_path));
    }
}
