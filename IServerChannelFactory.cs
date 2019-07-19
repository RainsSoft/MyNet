using MyNet.Channel;
using System;

namespace MyNet
{
    public interface IServerChannelFactory : IChannelFactory<ServerChannel>
    {
        TcpChannel CreateChild(ServerChannel parent, object message);
    }
}
