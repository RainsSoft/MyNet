using System;
namespace MyNet.Buffer
{
    public class PoolBufferAllocator : IStreamAllocator
    {
        public readonly static PoolBufferAllocator Default = new PoolBufferAllocator();
        protected IChunk _headChunk = null;
        protected int _maxbuffsize = 65536;
        /// <summary>
        /// 默认缓存大小
        /// </summary>
        public const int _buffsize = 8192;
        /// <summary>
        /// 缓存的最大倍数
        /// </summary>
        public const int MAX_BEI = 8;
        /// <summary>
        /// 每个chunk的缓存数
        /// </summary>
        public const int COUNT_PERCHUNK = 100;

        public PoolBufferAllocator()
        {
            _maxbuffsize = _buffsize * MAX_BEI;
        }

        public int MaxFrameBufferSize
        {
            get
            {
                return _maxbuffsize;
            }
            set
            {
                _maxbuffsize = value;
            }
        }

        protected void AddChunk(IChunk chunk)
        {
            if (_headChunk == null)
            {
                _headChunk = chunk;
                _headChunk.Pre = null;
                _headChunk.Next = null;
                return;
            }
            IChunk p = _headChunk;
            while (p.Next != null)
            {
                p = p.Next;
            }
            p.Next = chunk;
            chunk.Pre = p;
            chunk.Next = null;
        }
        public IChunk Alloc(int allocsize, out int offset)
        {
            lock (this)
            {
                offset = -1;
                int maxbuffsize = _buffsize * MAX_BEI;
                if (allocsize > maxbuffsize)
                {
                    IChunk bc = new BigChunk(allocsize);
                    offset = 0;
                    AddChunk(bc);
                    return bc;
                }
                else
                {
                    int needsize = allocsize;
                    if (allocsize % _buffsize != 0)
                    {
                        needsize = (allocsize / _buffsize + 1) * _buffsize;
                    }

                    IChunk p = _headChunk;
                    while (p != null)
                    {
                        if (p.BufferSize == needsize)
                        {
                            offset = p.AllocBuffer();
                            if (offset >= 0)
                            {
                                return p;
                            }
                        }
                        p = p.Next;
                    }

                    IChunk bc = new MinChunk(COUNT_PERCHUNK, needsize);
                    offset = bc.AllocBuffer();
                    AddChunk(bc);
                    return bc;
                }
            }
        }

        public IByteStream AllocStream(int allocsize)
        {
            int offset;
            IChunk cun = Alloc(allocsize, out offset);
            return new PoolIOStream(this, cun, offset);
        }
        public IByteStream AllocStream()
        {
            return AllocStream(_buffsize);
        }
    }
}
