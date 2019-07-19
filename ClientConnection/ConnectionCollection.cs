using System.Collections.Concurrent;

namespace MyNet.ClientConnection
{
    /// <summary>
    /// 空闲连接集
    /// </summary>
    public class ConnectionCollection
    {
        private const int MAX_COUNT = 10;
        /// <summary>
        /// 空闲连接
        /// </summary>
        private ConcurrentBag<IConnection> _idles = new ConcurrentBag<IConnection>();
        public int Count
        {
            get { return _idles.Count; }
        }
        public void Add(IConnection conn)
        {
            if (_idles.Count > MAX_COUNT)
            {
                _idles.Add(conn);
            }
        }
        public bool Take(out IConnection conn)
        {
            return _idles.TryTake(out conn);
        }
    }
}
