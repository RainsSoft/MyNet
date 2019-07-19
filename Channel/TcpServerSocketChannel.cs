using MyNet.Loop;
using System;
using System.Net.Sockets;

namespace MyNet.Channel
{
    /// <summary>
    /// 服务端接收连接的通道
    /// </summary>
    public class TcpServerSocketChannel : ServerChannel
    {

        private SyncTcpServerSocketChannel _tcpserversyncexe;
        protected override SyncChannel _syncexe
        {
            get
            {
                return _tcpserversyncexe;
            }
        }
        public TcpServerSocketChannel(Socket socket) : base(socket)
        {
            _tcpserversyncexe = new SyncTcpServerSocketChannel(this);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }
        public TcpServerSocketChannel() : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
        }
        public TcpServerSocketChannel(AddressFamily addressFamily) : this(new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp)) { }


        internal void Accept_Completed(object sender, SocketAsyncEventArgs eventArgs)
        {
            _loop.Execute(new DefaultRunnable<SocketAsyncEventArgs>(_tcpserversyncexe.ChannelReadTask, eventArgs));
        }

        protected override void OnAfterActive()
        {
            if (Loop.InCurrentThread())
            {
                _tcpserversyncexe.BeginAccept();
            }
            else
            {
                _loop.Execute(new SimpleRunnable(_tcpserversyncexe.BeginAccept));
            }

        }



        public class SyncTcpServerSocketChannel : SyncServerChannel
        {
            private SocketAsyncEventArgs _acceptOperation;
            private TcpServerSocketChannel _channel;
            public override ChannelBase Channel
            {
                get
                {
                    return _channel;
                }
            }
            public SyncTcpServerSocketChannel(TcpServerSocketChannel channel)
            {
                _channel = channel;
                _acceptOperation = new SocketAsyncEventArgs();
                _acceptOperation.Completed += new EventHandler<SocketAsyncEventArgs>(_channel.Accept_Completed);
            }
            public void BeginAccept()
            {
                if (_acceptOperation == null) return;
                bool willRaiseEvent = Channel.Config.ChannelSocket.AcceptAsync(_acceptOperation);
                if (!willRaiseEvent)
                {
                    ChannelReadTask(_acceptOperation);
                }
            }
            public void ChannelReadTask(SocketAsyncEventArgs op)
            {
                if (op.SocketError != SocketError.Success)
                {
                    return;
                }
                //触发ChannelRead事件
                Channel.Pipeline.FireChannelAccept(op.AcceptSocket);
                op.AcceptSocket = null;
                BeginAccept();
            }
            public override void Release()
            {
                base.Release();
                if (_acceptOperation != null)
                {
                    _acceptOperation.Dispose();
                    _acceptOperation = null;
                }
            }
        }
    }
}
