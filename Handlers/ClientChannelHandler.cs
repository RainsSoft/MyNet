using MyNet.Channel;
using System;

namespace MyNet.Handlers
{
    /// <summary>
    /// 首个客户端处理
    /// </summary>
    public class ClientChannelHandler : AbstractChannelHandler
    {
        protected IChannelFactory<TcpChannel> mFactory;
        public ClientChannelHandler(IChannelFactory<TcpChannel> factory)
        {
            mFactory = factory;
        }
  
        public override void ChannelInactive(IContext context)
        {
            context.FireNextInactive();
            mFactory.Release(context.Channel);
        }

        public override void HandlerInstalled(IContext context)
        {
        }

        public override void HandlerUninstalled(IContext context)
        {
        }
    }
}
