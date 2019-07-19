using System.Collections.Concurrent;


namespace MyNet.ClientConnection
{
    public class ConnectionManager
    {
        private static object _lock = new object();
        private volatile static ConnectionManager _instance;
     
        private ConcurrentDictionary<string, ConnectionCollection> _coll = new ConcurrentDictionary<string, ConnectionCollection>();
        public ConnectionManager()
        {
        }

        public static ConnectionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new ConnectionManager();
                    }
                }
                return _instance;
            }
        }


        /// <summary>
        /// 获取空闲连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public IConnection TakeIdle(string key)
        {
            ConnectionCollection cc = _coll.GetOrAdd(key, x =>
            {
                return new ConnectionCollection();
            });
            IConnection conn;
            if (cc.Take(out conn))
            {
                return conn;
            }

            return null;
        }
        /// <summary>
        /// 将空闲连接放入空闲集中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="conn"></param>
        public void ToIdle(string key, IConnection conn)
        {
            ConnectionCollection cc;
            if (_coll.TryGetValue(key, out cc))
            {
                cc.Add(conn);
            }
        }
    }
}
