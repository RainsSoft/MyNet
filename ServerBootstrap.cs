using MyNet.Channel;
using MyNet.Handlers;
using System.Net;

namespace MyNet
{
    /// <summary>
    /// 服务端启动类
    /// </summary>
    public class ServerBootstrap : AbstractBootstrap<ServerBootstrap, ServerChannel>
    {
        private IEventLoopGroup mChildGroup;
        private IChannelHandler mParentHandler;
        private IChannelHandler mChildHandler;
        private int mBacklog;

        public ServerBootstrap()
        {
            //默认
            mChannelFactory = new ServerChannelFactory();
            mBacklog = 10;
        }

        public override ServerBootstrap Group(IEventLoopGroup group)
        {
            return Group(group, group);
        }
        public ServerBootstrap Group(IEventLoopGroup group, IEventLoopGroup childgroup)
        {
            base.Group(group);
            mChildGroup = childgroup;
            return null;
        }
        public ServerBootstrap Backlog(int backlog)
        {
            mBacklog = backlog;
            return this;
        }
        public ServerBootstrap Handler(IChannelHandler handler)
        {
            mParentHandler = handler;
            mChildHandler = handler;
            return this;
        }
        public ServerBootstrap ChildHandler(IChannelHandler handler)
        {
            mChildHandler = handler;
            return this;
        }
        public ServerBootstrap ParentHandler(IChannelHandler handler)
        {
            mParentHandler = handler;
            return this;
        }
        protected override void OnUnManDisposed()
        {
            base.OnUnManDisposed();
            if (mChildGroup != null)
            {
                mChildGroup.Dispose();
            }
        }
        protected override void Init(ServerChannel channel, EndPoint localAddress)
        {
            channel.Bind(localAddress, mBacklog);
            if (mParentHandler != null)
            {
                channel.Pipeline.AddFirst(mParentHandler);
            }
            channel.Pipeline.AddFirst(new ServerChannelHandler((IServerChannelFactory)mChannelFactory, mChildGroup, mChildHandler));
            //激活Channel
            mParentGroup.Register(channel);
        }

    }
}
