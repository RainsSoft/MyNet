

using MyNet.Channel;

namespace MyNet.Handlers
{
    public class DefaultChannelHandlerContext : ChannleHandlerContext
    {
        private IChannelHandler mHandler;
        public DefaultChannelHandlerContext(ChannelBase channel, IChannelHandler handler) : base(channel)
        {
            mHandler = handler;
        }

        public override IChannelHandler Handler
        {
            get
            {
                return mHandler;
            }
        }
    }
}
