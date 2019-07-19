using MyNet.Channel;
using System;

namespace MyNet
{
    public interface IEventLoopGroup : IDisposable
    {
        void Register(ChannelBase channel);
    }
}
