
using System;

namespace MyNet.Middleware.Http
{
    public class HttpConfig
    {
        protected Func<HttpResponse,bool> _statusListener;
        /// <summary>
        /// 状态监听器,可监听处理非200状态
        /// 返回值false表示当前回复包不回复
        /// </summary>
        public Func<HttpResponse, bool> StatusListener
        {
            get { return _statusListener; }
            set { _statusListener = value; }
        }
        /// <summary>
        /// 服务端保持连接时间，单位秒
        /// </summary>
        protected int _keepTime = 60;
        public int KeepTime
        {
            get { return _keepTime; }
            set { _keepTime = value; }
        }
        /// <summary>
        /// Post长度限制2M,小于等于0表示不限制
        /// </summary>
        protected int _maxpostLen = 2097152;
        public int MaxPostLen
        {
            get { return _maxpostLen; }
        }
        protected int _maxUrlLen = 2048;
        public int MaxUrlLen
        {
            get { return _maxUrlLen; }
        }
        protected bool _disableGzip = false;
        /// <summary>
        /// 禁止Gzip压缩
        /// </summary>
        public bool DisableGzip
        {
            get { return _disableGzip; }
            set { _disableGzip = value; }
        }

        public HttpConfig() { }

    }
}
