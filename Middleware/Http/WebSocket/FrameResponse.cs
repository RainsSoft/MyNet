using MyNet.Channel;
using MyNet.Handlers;
using System;

namespace MyNet.Middleware.Http.WebSocket
{
    /// <summary>
    /// 表示WebSocket帧类型回复对象
    /// </summary>
    public class FrameResponse : WritePacket
    {
        /// <summary>
        /// 随机数
        /// </summary>
        public static readonly Random ran = new Random();

        /// <summary>
        /// 获取是否结束帧
        /// </summary>
        public bool Fin { get; private set; }

        /// <summary>
        /// 获取帧类型
        /// </summary>
        public FrameCodes Frame { get; private set; }

        /// <summary>
        /// 获取回复内容
        /// </summary>
        public byte[] Content { get; private set; }

        /// <summary>
        /// 构建不分片的回复帧
        /// </summary>
        /// <param name="frame">帧类型</param>
        /// <param name="content">内容</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FrameResponse(FrameCodes frame, byte[] content)
            : this(frame, content, true)
        {
        }

        /// <summary>
        /// 构建回复帧
        /// </summary>
        /// <param name="frame">帧类型</param>
        /// <param name="content">内容</param>
        /// <param name="fin">是否结束帧</param>
        /// <exception cref="ArgumentNullException"></exception>
        public FrameResponse(FrameCodes frame, byte[] content, bool fin)
        {
            this.Frame = frame;
            this.Fin = fin;
            this.Content = content ?? new byte[0];
        }


    }
}
