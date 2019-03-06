using LyricsFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var cont = @"
[00:12.00]Line 1 lyrics
[00:17.20][04:17.20]Line 2 lyrics
[00:21.10][01:17.20][02:17.20]Line 3 lyrics
[00:12.00]Line 4 lyrics
";

            var zz = LyricsParser.Parse(cont).OrderBy(x=>x.StartTime);

            foreach (var z in zz)
            {
                Console.WriteLine(z);
            }

            Console.ReadLine();
        }
    }
}
