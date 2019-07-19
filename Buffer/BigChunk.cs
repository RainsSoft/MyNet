using System;

namespace MyNet.Buffer
{
    public class BigChunk : AbstractChunk
    {
        private byte[] _buff;
        private int _buffsize;
        public override byte[] BufferBlock
        {
            get
            {
                return _buff;
            }
        }


        public override int BufferSize
        {
            get { return _buffsize; }
        }


        public BigChunk(int size)
        {
            _buffsize = size;
            _buff = new byte[size];
        }
        public override void FreeBuffer(int offset)
        {
            _buff = null;
            IChunk nc = this.Next;
            if (nc != null)
            {
                nc.Pre = this.Pre;
            }
            this.Pre.Next= nc;
            this.Pre = null;
            this.Next = null;
        }

        public override int AllocBuffer()
        {
            return -1;
        }
    }
}
