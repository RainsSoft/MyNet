using MyNet.Buffer;

namespace MyNet.Channel
{
    public class ReadPacket
    {
        protected int _readcount;
        protected IByteStream _stream;
        public IByteStream Stream
        {
            get { return _stream; }
        }
        public int ReadCount
        {
            get { return _readcount; }
            set { _readcount = value; }
        }
        public ReadPacket(IByteStream stream)
        {
            _stream = stream;
            
        }
     
    }
}
