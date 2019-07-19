using MyNet.Buffer;
using MyNet.Common;
using System;
using System.Linq;

namespace MyNet.Middleware.Http.WebSocket
{
    /// <summary>
    /// 表示WebSocket请求帧
    /// </summary>
    public class FrameRequest
    {
        /// <summary>
        /// 获取是否已完成
        /// </summary>
        public virtual bool Fin { get; private set; }

        /// <summary>
        /// 获取保存位
        /// </summary>
        public virtual ByteBits Rsv { get; private set; }

        /// <summary>
        /// 获取帧类型
        /// </summary>
        public virtual FrameCodes Frame { get; private set; }

        /// <summary>
        /// 获取是否有掩码
        /// </summary>
        public virtual bool Mask { get; private set; }

        /// <summary>
        /// 获取内容长度
        /// </summary>
        public virtual int ContentLength { get; private set; }

        /// <summary>
        /// 获取掩码
        /// </summary>
        public virtual byte[] MaskingKey { get; private set; }

        /// <summary>
        /// 获取请求帧的内容
        /// </summary>
        public virtual byte[] Content { get; private set; }

        public static FrameRequest CreateConnectedRequest()
        {
            return new FrameRequest
            {
                Fin = false,
                Rsv = 0,
                Mask = false,
                Frame = FrameCodes.Connected,
                ContentLength = 0,
                MaskingKey = new byte[0],
                Content = new byte[0]
            };
        }
        /// <summary>
        /// 解析请求的数据
        /// 返回请求数据包
        /// </summary>
        /// <param name="streamReader">数据读取器</param>  
        /// <param name="requiredMask">是否要求必须Mask</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static FrameRequest Parse(IByteStream streamReader, bool requiredMask = true)
        {
            if (streamReader.Length < 2)
            {
                return null;
            }

            ByteBits byte0 = streamReader.ReadByte();
            bool fin = byte0[0];
            ByteBits rsv = byte0.Take(1, 3);
            FrameCodes frameCode = (FrameCodes)(byte)byte0.Take(4, 4);

            ByteBits byte1 = streamReader.ReadByte();
            bool mask = byte1[0];
            if (requiredMask && mask == false)
            {
                throw new NotSupportedException("mask is required");
            }

            if (Enum.IsDefined(typeof(FrameCodes), frameCode) == false || rsv != 0)
            {
                throw new NotSupportedException();
            }

            int contentSize = 0;
            int contentLength = byte1.Take(1, 7);

            if (contentLength == 127)
            {
                contentSize = 8;
                contentLength = (int)streamReader.ReadLong();
            }
            else if (contentLength == 126)
            {
                contentSize = 2;
                contentLength = (ushort)streamReader.ReadShort();
            }

            int maskSize = mask ? 4 : 0;
            int packetLength = 2 + maskSize + contentSize + contentLength;
            if (streamReader.Length < packetLength)
            {
                return null;
            }

            byte[] maskingKey = mask ? streamReader.ReadBytes(4) : null;
            byte[] content = streamReader.ReadBytes(contentLength);

            if (mask && contentLength > 0)
            {
                for (int i = 0; i < contentLength; i++)
                {
                    content[i] = (byte)(content[i] ^ maskingKey[i % 4]);
                }
            }

            return new FrameRequest
            {
                Fin = fin,
                Rsv = rsv,
                Mask = mask,
                Frame = frameCode,
                ContentLength = contentLength,
                MaskingKey = maskingKey,
                Content = content
            };
        }
    }
}
