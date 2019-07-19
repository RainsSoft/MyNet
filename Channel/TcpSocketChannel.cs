using System;
using System.Net.Sockets;
using MyNet.Loop;
using System.Net;
using MyNet.Buffer;
using MyNet.Common;

namespace MyNet.Channel
{
    /// <summary>
    /// Tcp连接通道
    /// </summary>
    public class TcpSocketChannel : TcpChannel
    {
        private SyncTcpSocketChannel _tcpsyncexe;
        protected override SyncChannel _syncexe
        {
            get
            {
                return _tcpsyncexe;
            }
        }
        public TcpSocketChannel()
          : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
        }
        public TcpSocketChannel(Socket socket)
      : this(null, socket)
        {
        }
        public TcpSocketChannel(ServerChannel parent, Socket socket) : base(parent, socket)
        {
            _tcpsyncexe = new SyncTcpSocketChannel(this);
        }

        protected override void OnAfterActive()
        {
            if (Loop.InCurrentThread())
            {
                _tcpsyncexe.ConnectInit();
            }
            else
            {
                this.Loop.Execute(new SimpleRunnable(_tcpsyncexe.ConnectInit));
            }
        }


        internal void IO_Completed(object sender, SocketAsyncEventArgs eventArgs)
        {
            switch (eventArgs.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                case SocketAsyncOperation.ReceiveFrom:
                    _loop.Execute(new DefaultRunnable<SocketAsyncEventArgs>(_tcpsyncexe.FinishRead, eventArgs));
                    break;
                case SocketAsyncOperation.Send:
                case SocketAsyncOperation.SendTo:
                    _loop.Execute(new DefaultRunnable<SocketAsyncEventArgs>(_tcpsyncexe.FinishWrite, eventArgs));
                    break;
                case SocketAsyncOperation.Connect:
                    _loop.Execute(new DefaultRunnable<SocketAsyncEventArgs>(_tcpsyncexe.FinishConnect, eventArgs));
                    break;
            }

        }

        protected override bool DoConnect(EndPoint remoteAddress)
        {
            _remoteEndPoint = remoteAddress;
            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
            connectArgs.RemoteEndPoint = remoteAddress;
            connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

            return _config.ChannelSocket.ConnectAsync(connectArgs);
        }

        public class SyncTcpSocketChannel : SyncTcpChannel
        {
            private SocketAsyncEventArgs _readOp;
            private SocketAsyncEventArgs _writeOp;
            private ReadPacket _currentReadStreamOp;
            private TcpSocketChannel _channel;
            public override ChannelBase Channel
            {
                get
                {
                    return _channel;
                }
            }
            public SyncTcpSocketChannel(TcpSocketChannel channel)
            {
                _channel = channel;
                _readOp = new SocketAsyncEventArgs();
                _readOp.SetBuffer(new byte[0], 0, 0);
                _readOp.Completed += new EventHandler<SocketAsyncEventArgs>(_channel.IO_Completed);
                _currentReadStreamOp = new ReadPacket(PoolBufferAllocator.Default.AllocStream());
                _writeOp = new SocketAsyncEventArgs();
                _writeOp.SetBuffer(new byte[0], 0, 0);
                _writeOp.Completed += new EventHandler<SocketAsyncEventArgs>(_channel.IO_Completed);
            }

            public override void Release()
            {
                base.Release();
                if (_writeOp != null)
                {
                    _writeOp.Dispose();
                    _writeOp = null;
                }
                if (_readOp != null)
                {
                    _readOp.Dispose();
                    _readOp = null;
                }
            }
            public void FinishConnect(SocketAsyncEventArgs evargs)
            {
                if (evargs.SocketError != SocketError.Success)
                {
                    evargs.Dispose();
                    _channel.Dispose();
                }
                _channel._autoConnectEvent.Set();
            }


            public void FinishWrite(SocketAsyncEventArgs eventArgs)
            {
                if (eventArgs.SocketError != SocketError.Success)
                {
                    ErrorSendPackets();
                    _channel.Dispose();
                    return;
                }
                _currentWriteStreamOp.WriteCount += eventArgs.BytesTransferred;
                if (_currentWriteStreamOp.WriteCount == _currentWriteStreamOp.Stream.Length)
                {
                    _channel.Pipeline.FireChannelWriteFinish(_currentWriteStreamOp);
                    if (_currentWriteStreamOp != null)
                    {
                        _currentWriteStreamOp.Dispose();
                        _currentWriteStreamOp = null;
                    }
                    BeginWrite();
                }
                else
                {
                    DoWrite();
                }
            }
            protected override void DoWrite()
            {
                if (_currentWriteStreamOp == null)
                {
                    AgentLogger.Instance.Err("异常：当前写入流不存在");
                    return;
                }
                if (!_channel.Active)
                {
                    ErrorSendPackets();
                    return;
                }

                int curwritesize = _currentWriteStreamOp.Stream.Length - _currentWriteStreamOp.WriteCount;
                ArraySegment<byte> iobuf = _currentWriteStreamOp.Stream.GetIOBuffer();
                if (curwritesize > PoolBufferAllocator.Default.MaxFrameBufferSize)
                {
                    curwritesize = PoolBufferAllocator.Default.MaxFrameBufferSize;
                }

                _writeOp.SetBuffer(iobuf.Array, iobuf.Offset + _currentWriteStreamOp.WriteCount, curwritesize);
                bool willRaiseEvent = _channel.Config.ChannelSocket.SendAsync(_writeOp);
                if (!willRaiseEvent)
                {
                    FinishWrite(_writeOp);
                }
            }


            protected override void BeginRead()
            {
                if (!_channel.Active || _readOp == null) return;

                ArraySegment<byte> seg = _currentReadStreamOp.Stream.GetIOBuffer();
                int buffoffset = seg.Offset + _currentReadStreamOp.ReadCount;
                int buffcount = seg.Count - _currentReadStreamOp.ReadCount;
                _readOp.SetBuffer(seg.Array, buffoffset, buffcount);

                bool willRaiseEvent = _channel.Config.ChannelSocket.ReceiveAsync(_readOp);
                if (!willRaiseEvent)
                {
                    FinishRead(_readOp);
                }
            }

            public void FinishRead(SocketAsyncEventArgs evargs)
            {
                if (evargs.SocketError != SocketError.Success)
                {
                    _channel.Dispose();
                    return;
                }

                //获取接收的字节长度
                int lengthBuffer = evargs.BytesTransferred;
                //如果接收的字节长度为0，则判断远端服务器关闭连接
                if (lengthBuffer <= 0)
                {
                    //远端已经断开连接，禁止发送
                    _channel.Dispose();
                }
                else
                {
                    _currentReadStreamOp.ReadCount += lengthBuffer;
                    _currentReadStreamOp.Stream.SetWriterIndex(_currentReadStreamOp.ReadCount);
                    _channel.Pipeline.FireChannelRead(_currentReadStreamOp.Stream);
                    if (_mergeRead)
                    {
                        _currentReadStreamOp.Stream.EnsureWritable(PoolBufferAllocator.Default.MaxFrameBufferSize);
                        _mergeRead = false;
                    }
                    else
                    {
                        if (_currentReadStreamOp != null)
                        {
                            _currentReadStreamOp.Stream.Dispose();
                        }
                        _currentReadStreamOp = new ReadPacket(PoolBufferAllocator.Default.AllocStream());
                    }
                    BeginRead();
                }
            }


        }
    }
}
