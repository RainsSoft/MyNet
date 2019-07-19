using System;

namespace MyNet.Buffer
{
    public interface IChunk
    {
        IChunk Next { get; set; }
        IChunk Pre { get; set; }
        int BufferSize { get; }
        byte[] BufferBlock { get; }
        int AllocBuffer();
        void FreeBuffer(int offset);
    }
}
