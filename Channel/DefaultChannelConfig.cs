using MyNet.Buffer;
using System;
using System.Net.Sockets;
using System.Net;

namespace MyNet.Channel
{
    public class DefaultChannelConfig : IChannelConfig
    {
        protected Socket _socket;
        public DefaultChannelConfig(Socket socket)
        {
            _socket = socket;
        }
        public Socket ChannelSocket
        {
            get { return _socket; }
        }

    }
}
