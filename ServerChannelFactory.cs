using MyNet.Channel;
using System;
using System.Net;
using System.Net.Sockets;

namespace MyNet
{
    /// <summary>
    /// 默认ServerChannelFactory
    /// </summary>
    public class ServerChannelFactory : IServerChannelFactory
    {
        protected ChannelGroup _cg;
        public ChannelGroup CG
        {
            get { return _cg; }
        }
        public ServerChannelFactory()
        {
            _cg = new ChannelGroup();
        }
        public ServerChannel Create()
        {
            TcpServerSocketChannel socketchannel = new TcpServerSocketChannel();
            _cg.Add(socketchannel);
            return socketchannel;
        }

        public void Release(ChannelBase c)
        {
            _cg.Remove(c);
        }

        public TcpChannel CreateChild(ServerChannel parent, object message)
        {
            Socket connectedSocket = message as Socket;
            if (connectedSocket == null) return null;
            TcpSocketChannel channel = new TcpSocketChannel(parent, connectedSocket);
            _cg.Add(channel);
            return channel;
        }
        public void CloseServer()
        {

        }
    }
}
