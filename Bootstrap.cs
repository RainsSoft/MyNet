using MyNet.Channel;
using MyNet.Handlers;
using System.Net;
using System;

namespace MyNet
{
    /// <summary>
    /// 客户端启动类
    /// </summary>
    public class Bootstrap : AbstractBootstrap<Bootstrap, TcpChannel>
    {
        private IChannelHandler mHandler;
        public Bootstrap()
        {
            mChannelFactory = new ChannelFactory();
        }
        public Bootstrap Handler(IChannelHandler handler)
        {
            mHandler = handler;
            return this;
        }
        protected override void Init(TcpChannel channel, EndPoint localAddress)
        {
            channel.Bind(localAddress);
            if (mHandler != null)
            {
                channel.Pipeline.AddFirst(mHandler);
            }
            channel.Pipeline.AddFirst(new ClientChannelHandler(mChannelFactory));
            mParentGroup.Register(channel);
        }


    }
}
