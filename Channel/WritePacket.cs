using MyNet.Buffer;
using MyNet.Common;
using MyNet.Handlers;
using System;
namespace MyNet.Channel
{
    public delegate void WriteHandler(IContext context, WritePacket packet);
    public class WritePacket : BaseDisposable
    {
        /// <summary>
        /// 发送成功触发
        /// </summary>
        protected WriteHandler _successHandler;
        /// <summary>
        /// 发送失败触发
        /// </summary>
        protected WriteHandler _errHandler;
        protected int _writecount;
        protected IByteStream _stream;
        public IByteStream Stream
        {
            get { return _stream; }
        }
        public int WriteCount
        {
            get { return _writecount; }
            set { _writecount = value; }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public static WriteHandler Close = (IContext context, WritePacket packet) =>
        {
            context.Channel.Dispose();
        };
        public WritePacket Handler(WriteHandler handler)
        {
            _successHandler = handler;
            _errHandler = handler;
            return this;
        }
        public WritePacket SuccessHandler(WriteHandler handler)
        {
            _successHandler = handler;
            return this;
        }
        public WritePacket ErrHandler(WriteHandler handler)
        {
            _errHandler = handler;
            return this;
        }
        internal void EmitSuccess(IContext context)
        {
            _successHandler?.Invoke(context, this);
        }
        internal void EmitErr(IContext context)
        {
            _errHandler?.Invoke(context, this);
        }
        protected override void OnUnManDisposed()
        {
            _stream.Dispose();
            _successHandler = null;
            _errHandler = null;
        }


        public WritePacket()
        {
            _stream = PoolBufferAllocator.Default.AllocStream();
        }
    }
}
