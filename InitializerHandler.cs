using MyNet.Channel;
using MyNet.Handlers;
using System;

namespace MyNet
{
    /// <summary>
    /// 初始化专用
    /// </summary>
    public sealed class InitializerHandler : AbstractChannelHandler
    {
        Action<ChannelBase> _handler;
        public InitializerHandler(Action<ChannelBase> handler)
        {
            _handler = handler;
        }
        public override void ChannelActive(IContext context)
        {
            _handler(context.Channel);
            context.FireNextActive();
        }
        public override void HandlerInstalled(IContext context)
        {
        }

        public override void HandlerUninstalled(IContext context)
        {
        }
    }
}
