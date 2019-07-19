using MyNet.Channel;

namespace MyNet.ClientConnection
{
    public class Connection : IConnection
    {
        protected ChannelBase _channel;
        protected string _connkey;

        public Connection(string key, ChannelBase channel)
        {
            _connkey = key;
            _channel = channel;
        }

        public bool Active
        {
            get
            {
                return _channel.Active;
            }
        }
        /// <summary>
        /// 释放连接到空闲队列
        /// </summary>
        public void Close()
        {
            if (_channel.Active)
            {
                ConnectionManager.Instance.ToIdle(_connkey, this);
            }
        }
    }
}
