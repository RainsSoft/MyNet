using MyNet.Common;
using System;
using System.Text;

namespace MyNet.Middleware.Http.WebSocket
{
    public sealed class TextRequest : FrameRequest
    {
        /// <summary>
        /// 请求帧
        /// </summary>
        private readonly FrameRequest request;

        /// <summary>
        /// 获取请求帧的内容
        /// </summary>
        public override byte[] Content
        {
            get
            {
                return this.request.Content;
            }
        }


        /// <summary>
        /// 获取内容长度
        /// </summary>
        public override int ContentLength
        {
            get
            {
                return this.request.ContentLength;
            }
        }

        /// <summary>
        /// 获取是否已完成
        /// </summary>
        public override bool Fin
        {
            get
            {
                return this.request.Fin;
            }
        }

        /// <summary>
        /// 获取帧类型
        /// </summary>
        public override FrameCodes Frame
        {
            get
            {
                return this.request.Frame;
            }
        }

        /// <summary>
        /// 获取是否有掩码
        /// </summary>
        public override bool Mask
        {
            get
            {
                return this.request.Mask;
            }
        }

        /// <summary>
        /// 获取掩码
        /// </summary>
        public override byte[] MaskingKey
        {
            get
            {
                return this.request.MaskingKey;
            }
        }

        /// <summary>
        ///  获取保存位
        /// </summary>
        public override ByteBits Rsv
        {
            get
            {
                return this.request.Rsv;
            }
        }
        /// <summary>
        /// 获取Utf-8编码的内容
        /// </summary>
        public string Text
        {
            get
            {
                return Encoding.UTF8.GetString(this.request.Content);
            }
        }

        /// <summary>
        /// 文本请求帧
        /// </summary>
        /// <param name="request">请求帧</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public TextRequest(FrameRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }
            if (request.Frame != FrameCodes.Text)
            {
                throw new ArgumentException();
            }

            this.request = request;
        }
    }
}
