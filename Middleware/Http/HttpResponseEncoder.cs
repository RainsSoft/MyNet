
using MyNet.Common;
using MyNet.Middleware.Http.WebSocket;
using System;
using System.Text;
namespace MyNet.Middleware.Http
{
    public class HttpResponseEncoder
    {
        /// <summary>
        /// 输出回复内容
        /// 自动设置回复头的Content-Length
        /// </summary>      
        /// <param name="content">内容</param>
        /// <param name="gzip">gzip模式</param>
        public static void WriteResponse(HttpResponse response)
        {
            byte[] contentBytes = response.Content.ToArray();
            byte[] gzipbytes = null;
            if (response.Gzip && !response.HasGzip)
            {
                gzipbytes = Compression.GZipCompress(contentBytes);
                response.HasGzip = true;
            }
            else
            {
                gzipbytes = contentBytes;
            }


            byte[] headerBytes = GenerateResponseHeader(response, gzipbytes.Length, response.Gzip);
            byte[] buffer = ConcatBuffer(headerBytes, gzipbytes);

            response.Stream.WriteBytes(buffer);
            response.OutputStream.Dispose();
        }
        /// <summary>
        /// Websocket 握手输出
        /// </summary>
        /// <param name="response"></param>
        public static void WriteWebSocketHandshakeResponse(HandshakeResponse response)
        {
            HeaderBuilder builder = HeaderBuilder.NewResonse(101, "Switching Protocols");
            builder.Add("Upgrade", "websocket");
            builder.Add("Connection", "Upgrade");
            builder.Add("Sec-WebSocket-Accept", response.CreateResponseKey());
            builder.Add("Server", Utility.PRODUCT_NAME);
            response.Stream.WriteBytes(builder.ToByteArray());
        }
        /// <summary>
        /// WebSocket Frame输出
        /// </summary>
        /// <param name="response"></param>
        /// <param name="mask"></param>
        public static void WriteWebSocketFrameResponse(FrameResponse response, bool mask)
        {
            ByteBits bits = (byte)response.Frame;
            bits[0] = response.Fin;
            response.Stream.WriteByte(bits);
            if (response.Content.Length > UInt16.MaxValue)
            {
                response.Stream.WriteByte(mask ? byte.MaxValue : (byte)127);
                response.Stream.WriteLong((long)response.Content.Length);
            }
            else if (response.Content.Length > 125)
            {
                response.Stream.WriteByte(mask ? (byte)254 : (byte)126);
                response.Stream.WriteShort((short)response.Content.Length);
            }
            else
            {
                byte len = mask ? (byte)(response.Content.Length + 128) : (byte)response.Content.Length;
                response.Stream.WriteByte(len);
            }
            if (mask)
            {
                byte[] maskingKey = Converter.ToBytes(FrameResponse.ran.Next());
                response.Stream.WriteBytes(maskingKey);

                for (int i = 0; i < response.Content.Length; i++)
                {
                    response.Content[i] = (byte)(response.Content[i] ^ (maskingKey[i % 4]));
                }
            }

            response.Stream.WriteBytes(response.Content);
        }

        /// <summary>
        /// 生成头部数据
        /// </summary>
        /// <param name="contentLength">内容长度</param>
        /// <param name="gzip">gzip模式</param>
        /// <returns></returns>
        private static byte[] GenerateResponseHeader(HttpResponse response, int contentLength, bool gzip)
        {
            HeaderBuilder header = HeaderBuilder.NewResonse(response.Status, response.StatusDescription);
            header.Add("Date", DateTime.Now.ToUniversalTime().ToString("r"));
            header.Add("Server", Utility.PRODUCT_NAME);
            if (response.KeepAlive)
            {
                header.Add("Connection", "keep-alive");
            }
            else
            {
                header.Add("Connection", "close");
            }
            if (response.Charset == null)
            {
                header.Add("Content-Type", response.ContentType);
            }
            else
            {
                string contenType = string.Format("{0}; charset={1}", response.ContentType, response.Charset.WebName);
                header.Add("Content-Type", contenType);
            }

            if (contentLength >= 0)
            {
                header.Add("Content-Length", contentLength);
            }
            if (gzip == true)
            {
                header.Add("Content-Encoding", "gzip");
            }

            foreach (HttpCookie cookie in response.Cookies)
            {
                StringBuilder cookiesb = new StringBuilder();
                cookiesb.Append(string.Format("{0}={1}; ", cookie.Name, cookie.Value));
                if (cookie.Expires != -1)
                {
                    DateTime tt = Converter.Cast<DateTime>(cookie.Expires);
                    cookiesb.Append(string.Format("Expires={0}; ", tt.ToString("r")));
                }
                cookiesb.Append(string.Format("Path={0}; ", cookie.Path));
                cookiesb.Append(string.Format("Domain={0}; ", cookie.Domain));
                if (cookie.Secure)
                {
                    cookiesb.Append("Secure; ");
                }
                if (cookie.HttpOnly)
                {
                    cookiesb.Append("HttpOnly; ");
                }

                cookiesb.Remove(cookiesb.Length - 2, 2);
                header.Append("Set-Cookie", cookiesb.ToString());
            }

            foreach (string key in response.Headers.AllKeys)
            {
                header.Add(key, response.Headers[key]);
            }
            return header.ToByteArray();
        }


        /// <summary>
        /// 连接buffer
        /// </summary>
        /// <param name="buffer1"></param>
        /// <param name="buffer2"></param>
        /// <returns></returns>
        private static byte[] ConcatBuffer(byte[] buffer1, byte[] buffer2)
        {
            int length = buffer1.Length + buffer2.Length;
            byte[] buffer = new byte[length];
            System.Buffer.BlockCopy(buffer1, 0, buffer, 0, buffer1.Length);
            System.Buffer.BlockCopy(buffer2, 0, buffer, buffer1.Length, buffer2.Length);
            return buffer;
        }

    }
}
