using MyNet.Channel;
namespace MyNet.Handlers
{
    public class ServerChannelHandler : AbstractChannelHandler
    {
        protected IChannelHandler mChildHandler;
        protected IEventLoopGroup mChildLoopGroup;
        protected IServerChannelFactory mFactory;
        public ServerChannelHandler(IServerChannelFactory factory, IEventLoopGroup childLoopGroup, IChannelHandler childhandler)
        {
            mChildHandler = childhandler;
            mChildLoopGroup = childLoopGroup;
            mFactory = factory;
        }


        public override void ChannelAccept(IContext context, object accepter)
        {
            TcpChannel childchannel = mFactory.CreateChild(context.Channel as ServerChannel, accepter);
            if (mChildHandler != null)
            {
                childchannel.Pipeline.AddFirst(mChildHandler);
            }
            childchannel.Pipeline.AddFirst(this);
            mChildLoopGroup.Register(childchannel);
            context.FireNextAccept(accepter);
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
