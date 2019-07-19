using System.Net;
using System.Net.Sockets;

namespace MyNet.Channel
{
    public abstract class ServerChannel : AbstractChannel
    {
        protected EndPoint _localIpPoint;

        public override EndPoint LocalEndPoint
        {
            get
            {
                return _localIpPoint;
            }
        }
        protected int _backlog;
        public override EndPoint RemoteEndPoint
        {
            get
            {
                return _localIpPoint;
            }
        }
 
        public ServerChannel(Socket socket) : base(socket)
        {
        }
        public void Bind(EndPoint localAddress, int backlog)
        {
            _localIpPoint = localAddress;
            _backlog = backlog;
        }
        protected override void OnBeforeActive()
        {
            _config.ChannelSocket.Bind(_localIpPoint);
            _config.ChannelSocket.Listen(_backlog);
        }


        public abstract class SyncServerChannel : AbstractSyncChannel
        {
            public override void Release()
            {
                if (Channel.Config.ChannelSocket.Connected)
                {
                    Channel.Config.ChannelSocket.Close();
                }
                base.Release();
            }
            public override void MergeRead() { }
            public override void WriteAsync(object packet){}
        }
    }

}
