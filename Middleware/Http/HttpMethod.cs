using System;

namespace MyNet.Middleware.Http
{
    /// <summary>
    /// 表示请求方式   
    /// </summary>
    [Flags]
    public enum HttpMethod
    {
        /// <summary>
        /// Get
        /// </summary>
        GET = 0x1,

        /// <summary>
        /// Post
        /// </summary>
        POST = 0x2,

        /// <summary>
        /// PUT
        /// </summary>
        PUT = 0x4,

        /// <summary>
        /// 用于请求获得由Request-URI标识的资源在请求/响应的通信过程中可以使用的功能选项
        /// </summary>
        OPTIONS = 0x5,

        /// <summary>
        /// DELETE
        /// </summary>
        DELETE = 0x8,
    }
}
