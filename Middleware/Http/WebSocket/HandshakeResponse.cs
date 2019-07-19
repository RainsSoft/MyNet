using MyNet.Channel;
using MyNet.Handlers;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MyNet.Middleware.Http.WebSocket
{
    /// <summary>
    /// 表示Websocket的握手回复
    /// </summary>
    public class HandshakeResponse : WritePacket
    {
        private const string sha1guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        /// <summary>
        /// Sec-WebSocket-Key
        /// </summary>
        private string secValue;
        private HttpRequest request;
        public HttpRequest Request
        {
            get { return request; }
        }
        /// <summary>
        /// 表示握手回复
        /// </summary>
        /// <param name="secValue">Sec-WebSocket-Key</param>
        public HandshakeResponse(HttpRequest req, string secValue)
        {
            this.secValue = secValue;
            this.request = req;
        }

        /// <summary>
        /// 生成回复的key
        /// </summary>      
        /// <returns></returns>
        public string CreateResponseKey()
        {

            byte[] bytes = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(this.secValue + sha1guid));
            return Convert.ToBase64String(bytes);
        }

    }
}
