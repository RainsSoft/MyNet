using System;
using System.Net.Sockets;

namespace MyNet.Channel
{
    public interface IChannelConfig
    {
        Socket ChannelSocket { get; }
    }
}
