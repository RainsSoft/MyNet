using System;
using System.Threading;
namespace MyNet.Channel
{
    public class ChannelID : IComparable<ChannelID>
    {
        private long mid;
        private static long InitSeed = DateTime.Now.Ticks & 0xFFFFFFFF;

        public ChannelID()
        {
            mid = Interlocked.Increment(ref InitSeed);
        }

        public override string ToString()
        {
            return mid.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj == this) return true;
            ChannelID id = obj as ChannelID;
            if (id == null) return false;
            return Equals(this.mid, id.mid);
        }
        public override int GetHashCode()
        {
            return mid.GetHashCode();
        }
        public int CompareTo(ChannelID other)
        {
            return mid.CompareTo(other.mid);
        }


    }
}
