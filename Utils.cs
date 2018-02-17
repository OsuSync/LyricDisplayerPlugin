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
        public static bool DebugMode { get; internal set; } = false;

        public static bool EnableOutputSearchResult { get; internal set; } = false;

        public static bool PreferTranslateLyrics { get; internal set; } = false;

        public static void Output(string message, ConsoleColor color, bool new_line = true, bool time = true)
        {
            IO.CurrentIO.WriteColor("[LyricDisplayer]" + message, color, new_line, time);
        }

        public static void Debug(string message, bool new_line = true, bool time = true)
        {
            if (DebugMode)
                Output(message, ConsoleColor.Cyan, new_line, time);
        }

        //https://www.programcreek.com/2013/12/edit-distance-in-java/
        public static int EditDistance(string a,string b)
        {
            int len_a = a.Length;
            int len_b = b.Length;
            
            int[,] dp = new int[len_a + 1,len_b + 1];

            for (int i = 0; i <= len_a; i++)
            {
                dp[i,0] = i;
            }

            for (int j = 0; j <= len_b; j++)
            {
                dp[0,j] = j;
            }
            
            for (int i = 0; i < len_a; i++)
            {
                char c_a = a[i];
                for (int j = 0; j < len_b; j++)
                {
                    char c_b = b[j];
                    
                    if (c_a == c_b)
                    {
                        dp[i + 1,j + 1] = dp[i,j];
                    }
                    else
                    {
                        int replace = dp[i,j] + 1;
                        int insert = dp[i,j + 1] + 1;
                        int delete = dp[i + 1,j] + 1;

                        int min = Math.Min(Math.Min(insert , replace),delete);
                        dp[i + 1,j + 1] = min;
                    }
                }
            }

            return dp[len_a,len_b];
        }
    }
}
