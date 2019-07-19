
using MyNet.Channel;

namespace MyNet.Handlers
{
    public class HeadContext : ChannleHandlerContext, IChannelHandler
    {
        public HeadContext(ChannelBase channel) : base(channel)
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
            //真正的发送数据
            Channel.WriteAsync(msg);
        }

        public void ChannelWriteErr(IContext context, object msg)
        {
        }

        public void ChannelWriteFinish(IContext context, object msg)
        {
        }
        public void HandlerInstalled(IContext context)
        {
        }

        public void HandlerUninstalled(IContext context)
        {
        }
    }
}
