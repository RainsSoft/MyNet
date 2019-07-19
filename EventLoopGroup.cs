using MyNet.Channel;
using MyNet.Common;
using MyNet.Loop;
using System;
using System.Threading;

namespace MyNet
{
    public class EventLoopGroup : BaseDisposable, IEventLoopGroup
    {
        protected long _index;
        protected int mMaxLoop;
        protected IEventLoop[] mEventLoops;
        public EventLoopGroup() : this(1)
        {
            _index = 0;
        }
        public EventLoopGroup(int maxloop)
        {
            mMaxLoop = maxloop;
            mEventLoops = new IEventLoop[mMaxLoop];
            for (int i = 0; i < mMaxLoop; i++)
            {
                IEventLoop eventLoop = new EventLoop(this);
                mEventLoops[i] = eventLoop;
            }
        }
        /// <summary>
        /// 将IChannel绑定到IEventLoop
        /// </summary>
        /// <param name="channel"></param>
        public void Register(ChannelBase channel)
        {
            IEventLoop loop = mEventLoops[_index % mMaxLoop];
            Interlocked.Increment(ref _index);
            channel.RegisterLoop(loop);
        }

        protected override void OnUnManDisposed()
        {
            for (int i = 0; i < mMaxLoop; i++)
            {
                mEventLoops[i].Close();
            }
        }
    }
}
