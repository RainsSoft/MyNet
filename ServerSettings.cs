using System;
using System.Collections.Generic;
using System.Net;

namespace MyNet
{
    public class ServerSettings
    {
        private int mMaxConn;
        /// <summary>
        /// 可连接量
        /// </summary>
        public int MaxConn
        {
            get { return mMaxConn; }
        }
        private int mMaxAccept;
        /// <summary>
        /// 可接受量
        /// </summary>
        public int MaxAccept
        {
            get { return mMaxAccept; }
        }
        private int mBuffSize;
        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public int BuffSize
        {
            get { return mBuffSize; }
        }
        private int mBacklog;
        /// <summary>
        /// 队列最大等待个数
        /// </summary>
        public int Backlog
        {
            get { return mBacklog; }
        }
        private IPEndPoint mLocalEndPoint;
        /// <summary>
        /// 监听的Ip和端口
        /// </summary>

        public IPEndPoint LocalEndPoint
        {
            get { return mLocalEndPoint; }
        }
        /// <summary>
        /// 默认服务器参数
        /// </summary>
        public ServerSettings() : this(10000, 1000, 8192, 1000, 80) { }
        public ServerSettings(int maxconn, int maxaccept, int bufsize, int backlog, int port)
        {
            mMaxConn = maxconn;
            mMaxAccept = maxaccept;
            mBuffSize = bufsize;
            mBacklog = backlog;
            mLocalEndPoint = new IPEndPoint(IPAddress.Any, port);
        }
    }
}
