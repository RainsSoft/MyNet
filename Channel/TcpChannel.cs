using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System;

namespace MyNet.Channel
{
    public abstract class TcpChannel : AbstractChannel
    {
        internal AutoResetEvent _autoConnectEvent;
        protected ServerChannel _parent;
        protected EndPoint _remoteEndPoint;
        protected EndPoint _localEndPoint;

        public override EndPoint RemoteEndPoint
        {
            get { return _remoteEndPoint; }
        }
        public override EndPoint LocalEndPoint
        {
            get { return _localEndPoint; }
        }

        public TcpChannel(ServerChannel parent, Socket socket) : base(socket)
        {
            if (parent != null)
            {
                _parent = parent;
                _localEndPoint = parent.LocalEndPoint;
            }
            else
            {
                _localEndPoint = socket.LocalEndPoint;
            }

            _autoConnectEvent = new AutoResetEvent(false);
            _remoteEndPoint = socket.RemoteEndPoint as IPEndPoint;
        }



        /// <summary>
        /// 绑定连接端口
        /// </summary>
        /// <param name="remoteAddress"></param>
        public void Bind(EndPoint remoteAddress)
        {
            _remoteEndPoint = remoteAddress;
        }
        protected override void OnBeforeActive()
        {
            if (_parent == null)
            {
                bool issuccess = DoConnect(_remoteEndPoint);
                _autoConnectEvent.WaitOne();
            }
        }

        protected abstract bool DoConnect(EndPoint remoteAddress);

        public abstract class SyncTcpChannel : AbstractSyncChannel
        {
            protected bool _mergeRead;
            protected Queue<WritePacket> _streamQueue;
            protected WritePacket _currentWriteStreamOp;
            public SyncTcpChannel()
            {
                _mergeRead = false;
                _streamQueue = new Queue<WritePacket>();
            }
            public override void MergeRead()
            {
                _mergeRead = true;
            }
            public override void Release()
            {
                try
                {
                    if (Channel.Config.ChannelSocket.Connected)
                    {
                        Channel.Config.ChannelSocket.Shutdown(SocketShutdown.Both);
                        Channel.Config.ChannelSocket.Close();
                    }
                }
                catch{}

                base.Release();
            }


            /// <summary>
            /// 异步发送数据
            /// </summary>
            /// <param name="op"></param>
            public override void WriteAsync(object packet)
            {
                WritePacket tpacket = packet as WritePacket;
                if (tpacket == null)
                {
                    Common.AgentLogger.Instance.Info("WriteAsync发送的数据包格式错误");
                    return;
                }
                _streamQueue.Enqueue(tpacket);
                if (_currentWriteStreamOp == null)
                {
                    BeginWrite();
                }
            }
            public void ConnectInit()
            {
                BeginRead();
                BeginWrite();
            }
            /// <summary>
            /// 发送下一个数据包
            /// </summary>
            protected void BeginWrite()
            {
                if (_streamQueue.Count > 0)
                {
                    _currentWriteStreamOp = _streamQueue.Dequeue();
                    DoWrite();
                }
            }
            /// <summary>
            /// 所有未发送的数据触发发送失败
            /// </summary>
            protected void ErrorSendPackets()
            {
                while (_currentWriteStreamOp != null)
                {
                    Channel.Pipeline.FireChannelWriteErr(_currentWriteStreamOp);
                    _currentWriteStreamOp.Dispose();
                    _currentWriteStreamOp = null;
                    if (_streamQueue.Count > 0)
                    {
                        _currentWriteStreamOp = _streamQueue.Dequeue();
                    }
                }

            }

            protected abstract void DoWrite();
            protected abstract void BeginRead();
        }
    }
}
