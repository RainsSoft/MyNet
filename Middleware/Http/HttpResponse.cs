
using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Handlers;
using System;
using System.IO;
using System.Text;
namespace MyNet.Middleware.Http
{
    /// <summary>
    /// 表示Http回复对象
    /// </summary>
    public class HttpResponse : WritePacket
    {
        public HttpCookieCollection Cookies { get; internal set; }
        public bool KeepAlive { get; set; }

        /// <summary>
        /// 获取或设置Http状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 获取或设置输出的 HTTP 状态字符串
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// 获取或设置内容体的编码
        /// </summary>
        public Encoding Charset { get; set; }

        /// <summary>
        /// 获取回复头信息
        /// </summary>
        public HttpNameValueCollection Headers { get; private set; }

        /// <summary>
        /// 获取或设置Header的内容类型
        /// </summary>
        public string ContentType
        {
            get
            {
                return this.Headers["Content-Type"];
            }
            set
            {
                this.Headers["Content-Type"] = value;
            }
        }

        /// <summary>
        /// 获取或设置Header的内容描述
        /// </summary>
        public string ContentDisposition
        {
            get
            {
                return this.Headers["Content-Disposition"];
            }
            set
            {
                this.Headers["Content-Disposition"] = value;
            }
        }
        public bool Gzip { get; set; }
        /// <summary>
        /// 表示数据是否已经压缩过
        /// </summary>
        public bool HasGzip { get; set; }
        public IByteStream Content { get; }
        public Stream OutputStream { get; }
        public void Clear()
        {
            Content.Clear();
            Content.SetWriterIndex(0);
        }
        public void Write(byte[] data)
        {
            Content.WriteBytes(data);
        }
        public void Write(string data)
        {
            Content.WriteBytes(Charset.GetBytes(data));
        }
        public void End400(IContext context)
        {
            this.Status = 400;
            this.StatusDescription = "Bad Request";
            context.Channel.SendAsync(this);
        }
        public void End401(IContext context)
        {
            this.Status = 401;
            this.StatusDescription = "Unauthorized";
            context.Channel.SendAsync(this);
        }
        public void End404(IContext context)
        {
            this.Status = 404;
            this.StatusDescription = "Not Found";
            context.Channel.SendAsync(this);
        }
        /// <summary>
        /// 请求超时
        /// </summary>
        /// <param name="context"></param>
        public void End408(IContext context)
        {
            this.Status = 408;
            this.StatusDescription = "Request Timeout";
            context.Channel.SendAsync(this);
        }
        /// <summary>
        /// 请求实体太大
        /// </summary>
        public void End413(IContext context)
        {
            this.Status = 413;
            this.StatusDescription = "Request Entity Too Large";
            context.Channel.SendAsync(this);
        }
        /// <summary>
        /// 请求URI太长
        /// </summary>
        public void End414(IContext context)
        {
            this.Status = 414;
            this.StatusDescription = "Request URI Too Long";
            context.Channel.SendAsync(this);
        }
        /// <summary>
        /// 断点续传下载返回
        /// </summary>
        public void End206(IContext context)
        {
            this.Status = 206;
            this.StatusDescription = "Partial Content";
            context.Channel.SendAsync(this);
        }
        /// <summary>
        /// 请求执行成功，但是没有数据，浏览器不用刷新页面.也不用导向新的页面
        /// </summary>
        /// <param name="context"></param>
        public void End204(IContext context)
        {
            this.Status = 204;
            this.StatusDescription = "No Content";
            context.Channel.SendAsync(this);
        }
        /// <summary>
        /// 断点续传失效返回
        /// </summary>
        public void End416(IContext context)
        {
            this.Status = 416;
            this.StatusDescription = "Request Range Not Satisfiable";
            context.Channel.SendAsync(this);
        }

        public void End500(IContext context)
        {
            this.Status = 500;
            this.StatusDescription = "Internal Server Error";
            context.Channel.SendAsync(this);
        }
        /// <summary>
        /// 临时的服务器维护或者过载,服务器超时
        /// </summary>
        /// <param name="context"></param>
        public void End503(IContext context)
        {
            this.Status = 503;
            this.StatusDescription = "Service Unavailable";
            context.Channel.SendAsync(this);
        }
        public void End(IContext context)
        {
            context.Channel.SendAsync(this);
        }
        public void Redirect(IContext context, string url, bool end)
        {
            this.Status = 301;
            this.Headers.Add("Location", url);
            if (end)
            {
                context.Channel.SendAsync(this);
            }
        }
        /// <summary>
        /// 表示http回复
        /// </summary>
        /// <param name="session">会话</param>
        public HttpResponse()
        {
            this.KeepAlive = true;
            this.Content = PoolBufferAllocator.Default.AllocStream();
            this.OutputStream = new CSharpStream(this.Content);
            this.Gzip = true;
            this.HasGzip = false;
            this.Cookies = new HttpCookieCollection();
            this.Charset = Encoding.UTF8;
            this.Status = 200;
            this.StatusDescription = "OK";
            this.Headers = new HttpNameValueCollection();
            this.ContentType = "text/html";
        }
    }
}
