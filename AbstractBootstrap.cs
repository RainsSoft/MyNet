using MyNet.Channel;
using MyNet.Common;
using System.Net;

namespace MyNet
{
    public abstract class AbstractBootstrap<TBoot, TChannel> : BaseDisposable
        where TBoot : AbstractBootstrap<TBoot, TChannel>
        where TChannel : ChannelBase
    {
        protected IEventLoopGroup mParentGroup;
        protected IChannelFactory<TChannel> mChannelFactory;
        public AbstractBootstrap()
        {
            AgentLogger.Instance.Logo();
        }
        /// <summary>
        /// 绑定循环组
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual TBoot Group(IEventLoopGroup g)
        {
            mParentGroup = g;
            return (TBoot)this;
        }
        public IChannelFactory<TChannel> Factory()
        {
            return mChannelFactory;
        }
        public TBoot Channel(IChannelFactory<TChannel> factory)
        {
            mChannelFactory = factory;
            return (TBoot)this;
        }
        public TChannel LaunchChannel(string ip, int port)
        {
            return LaunchChannel(IPAddress.Parse(ip), port);
        }
        /// <summary>
        /// 创建并启动Channel
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>

        public TChannel LaunchChannel(IPAddress inetHost, int inetPort)
        {
            //初始化Channel
            TChannel channel = mChannelFactory.Create();
            Init(channel, new IPEndPoint(inetHost, inetPort));
            return channel;
        }
        protected override void OnUnManDisposed()
        {
            if (mParentGroup != null)
            {
                mParentGroup.Dispose();
                mParentGroup = null;
            }
        }
        protected abstract void Init(TChannel channel, EndPoint localAddress);
    }
}
