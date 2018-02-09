using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricDisplayerPlugin
{
    public struct Sentence : IEquatable<Sentence>, IComparable<Sentence>
    {
        public static Sentence Empty { get; private set; } = new Sentence(string.Empty, -1);

        public int StartTime { get; private set; }
        public string Content { get; private set; }

        public Sentence(string content, int start_time)
        {
            this.StartTime = start_time;
            this.Content = content;
        }

        public bool Equals(Sentence other)
        {
            return StartTime == other.StartTime && other.Content == Content;
        }

        public int CompareTo(Sentence other)
        {
            return StartTime - other.StartTime;
        }

        public override string ToString()
        {
            return $"{StartTime} {Content}";
        }
    }
}
