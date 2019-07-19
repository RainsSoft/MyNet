using System;

namespace MyNet.Buffer
{
    public abstract class AbstractChunk : IChunk
    {
        protected IChunk _next = null;
        protected IChunk _pre = null;

        public IChunk Next
        {
            get
            {
                return _next;
            }
            set
            {
                _next = value;
            }
        }

        public IChunk Pre
        {
            get
            {
                return _pre;
            }
            set
            {
                _pre = value;
            }
        }


        public abstract byte[] BufferBlock { get; }
        public abstract int BufferSize { get; }
        public abstract int AllocBuffer();
        public abstract void FreeBuffer(int offset);
    }
}
