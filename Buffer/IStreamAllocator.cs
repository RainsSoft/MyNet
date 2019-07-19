using System;

namespace MyNet.Buffer
{
    public interface IStreamAllocator
    {
        int MaxFrameBufferSize { get; }
        /// <summary>
        /// 创建Stream
        /// </summary>
        /// <param name="size"></param>
        IByteStream AllocStream(int allocsize);
        /// <summary>
        /// 默认初始大小创建Stream
        /// </summary>
        /// <returns></returns>
        IByteStream AllocStream();
    }
}
