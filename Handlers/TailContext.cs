using MyNet.Channel;

namespace MyNet.Handlers
{
    public class TailContext : ChannleHandlerContext, IChannelHandler
    {
        public TailContext(ChannelBase channel) : base(channel)
        {
        }

        public override IChannelHandler Handler
        {
            get
            {
                return this;
            }
        }

        public void ChannelAccept(IContext context, object accepter)
        {
        }

        public void ChannelActive(IContext context)
        {
        }

        public void ChannelInactive(IContext context)
        {
        }

        public void ChannelRead(IContext context, object msg)
        {
        }

        public void ChannelWrite(IContext context, object msg)
        {
        }
        /// <summary>
        /// 触发写入包的发送失败事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        public void ChannelWriteErr(IContext context, object msg)
        {
            WritePacket packet = msg as WritePacket;
            if (packet != null)
            {
                packet.EmitErr(context);
            }
        }
        /// <summary>
        /// 触发写入包的发送成功事件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="msg"></param>
        public void ChannelWriteFinish(IContext context, object msg)
        {
            WritePacket packet = msg as WritePacket;
            if (packet != null)
            {
                packet.EmitSuccess(context);
            }
        }
        public void HandlerInstalled(IContext context)
        {
        }

        public void HandlerUninstalled(IContext context)
        {
        }
    }
}
