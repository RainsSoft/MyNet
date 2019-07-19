using System;
using MyNet.Channel;
using MyNet.Common;

namespace MyNet.ClientConnection
{
    public abstract class AbstractConnectionFactory<T> : BaseDisposable where T : class, IConnection
    {
        protected string _ip;
        protected int _port;
        protected string _ns;
        protected Bootstrap _bootstrap;
        protected EventLoopGroup _workGroup;
        public AbstractConnectionFactory(string ip, int port, string ns)
        {
            _ip = ip;
            _port = port;
            _ns = ns;
            _workGroup = new EventLoopGroup(4);
            _bootstrap = new Bootstrap();
            _bootstrap.Group(_workGroup);
        }

        public T NewConnection()
        {
            string key = string.Format("{0}:{1}/{2}", _ip, _port, _ns);
            T conn = ConnectionManager.Instance.TakeIdle(key) as T;
            if (conn == null || !conn.Active)
            {
                return NewConnection(key, _bootstrap.LaunchChannel(_ip, _port));
            }
            return conn;
        }
        protected override void OnUnManDisposed()
        {
            _workGroup.Dispose();
            _workGroup = null;
            _bootstrap = null;
        }
        public abstract T NewConnection(string key, ChannelBase channel);
    }
}
