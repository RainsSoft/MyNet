using System;
using System.IO;

namespace MyNet.Buffer
{
    public class PoolIOStream : AbstractByteStream
    {
        protected IChunk _chunk;
        protected int _chunkoffset;
        protected PoolBufferAllocator _allocator;
        public override int Capacity
        {
            get { return _chunk.BufferSize; }
        }


        public PoolIOStream(PoolBufferAllocator allocator, IChunk chunk, int offset) :
            this(allocator, chunk, offset, 0)
        {
        }
        public PoolIOStream(PoolBufferAllocator allocator, IChunk chunk, int offset, int initlen)
        {
            _readerindex = 0;
            _writerindex = initlen;
            _allocator = allocator;
            _chunk = chunk;
            _chunkoffset = offset;
        }

        public override IByteStream AdjustCapacity(int newCapacity)
        {
            int offset;
            IChunk chunk = _allocator.Alloc(newCapacity, out offset);
            if (_writerindex > 0)
            {
                System.Buffer.BlockCopy(_chunk.BufferBlock, _chunkoffset, chunk.BufferBlock, offset, _writerindex);
            }
            _chunk.FreeBuffer(_chunkoffset);
            _chunk = chunk;
            _chunkoffset = offset;
            return this;
        }
        public override IByteStream ToBase64()
        {
            string base64str = Convert.ToBase64String(this.ToArray());
            byte[] base64arr = Convert.FromBase64String(base64str);
            int offset;
            IChunk chunk = _allocator.Alloc(base64arr.Length, out offset);
            System.Buffer.BlockCopy(base64arr, 0, chunk.BufferBlock, offset, base64arr.Length);
            return new PoolIOStream(_allocator, chunk, offset);
        }
        public override IByteStream Slice(int start, int len)
        {
            int offset;
            IChunk chunk = _allocator.Alloc(len, out offset);
            if ((start + len) > Length)
            {
                return null;
            }
            System.Buffer.BlockCopy(_chunk.BufferBlock, _chunkoffset + start, chunk.BufferBlock, offset, len);
            return new PoolIOStream(_allocator, chunk, offset, len);
        }
        public override byte GetByte(int index)
        {
            return _chunk.BufferBlock[_chunkoffset + index];
        }
        public override byte[] GetBytes(int start, int len)
        {
            byte[] newBytes = new byte[len];
            System.Buffer.BlockCopy(_chunk.BufferBlock, _chunkoffset + start, newBytes, 0, len);
            return newBytes;
        }
        public override void SetBytes(int start, byte[] bs, int offset, int len)
        {
            System.Buffer.BlockCopy(bs, offset, _chunk.BufferBlock, _chunkoffset + start, len);
        }
        public override void SetByte(int start, byte b)
        {
            _chunk.BufferBlock[_chunkoffset + start] = b;
        }

        public override ArraySegment<byte> GetIOBuffer()
        {
            return new ArraySegment<byte>(_chunk.BufferBlock, _chunkoffset, _chunk.BufferSize);
        }
        public override void Clear()
        {
            Array.Clear(_chunk.BufferBlock, _chunkoffset, _chunk.BufferSize);
        }

        protected override void OnUnManDisposed()
        {
            _readerindex = 0;
            _writerindex = 0;
            _chunk.FreeBuffer(_chunkoffset);
        }

    }
}
