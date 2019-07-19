using MyNet.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MyNet.Middleware.Http
{
    /// <summary>
    /// 表示Http请求信息
    /// </summary>
    public class HttpRequest
    {
        public HttpCookieCollection Cookies { get; internal set; }
        /// <summary>
        /// 获取请求的头信息
        /// </summary>
        public HttpNameValueCollection Headers { get; internal set; }

        /// <summary>
        /// 获取Query
        /// </summary>
        public HttpNameValueCollection Query { get; internal set; }

        /// <summary>
        /// 获取Form 
        /// </summary>
        public HttpNameValueCollection Form { get; internal set; }

        /// <summary>
        /// 获取请求的文件
        /// </summary>
        public HttpFile[] Files { get; internal set; }

        /// <summary>
        /// 获取Post的内容
        /// </summary>
        public byte[] Body { get; internal set; }
        public Stream InputStream { get; internal set; }

        /// <summary>
        /// 获取请求方法
        /// </summary>
        public HttpMethod HttpMethod { get; internal set; }

        /// <summary>
        /// 获取请求路径
        /// </summary>
        public string Path { get; internal set; }
        /// <summary>
        /// 主机名
        /// </summary>
        public string Host { get; internal set; }

        /// <summary>
        /// 获取请求的Uri
        /// </summary>
        public Uri Url { get; internal set; }
        /// <summary>
        /// 浏览器向 WEB 服务器表明自己是从哪个 网页/URL 获得/点击 当前请求中的网址/URL。 
        /// </summary>
        public Uri RefererUrl { get; internal set; }
        /// <summary>
        /// 浏览器表明自己的身份（是哪种浏览器）
        /// </summary>
        public string UserAgent { get; internal set; }

        /// <summary>
        /// 获取监听的本地IP和端口
        /// </summary>
        public IPEndPoint LocalEndPoint { get; internal set; }

        /// <summary>
        /// 获取远程端的IP和端口
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; internal set; }


        /// <summary>
        /// 从Query和Form获取请求参数的值
        /// 多个值会以逗号分隔
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (this.Query.ContainsKey(key))
                {
                    return this.Query[key];
                }
                else
                {
                    return this.Form[key];
                }
            }
        }

        /// <summary>
        /// Http请求信息
        /// </summary>
        internal HttpRequest()
        {
            Cookies = new HttpCookieCollection();
        }
        /// <summary>
        /// 从Query和Form获取请求参数的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public IList<string> GetValues(string key)
        {
            string[] queryValues = this.Query.GetValues(key);
            string[] formValues = this.Form.GetValues(key);

            List<string> list = new List<string>();
            if (queryValues != null)
            {
                list.AddRange(queryValues);
            }
            if (formValues != null)
            {
                list.AddRange(formValues);
            }
            return list;
        }


        /// <summary>
        /// 是否为ajax请求
        /// </summary>
        /// <returns></returns>
        public bool IsAjaxRequest()
        {
            return this["X-Requested-With"] == "XMLHttpRequest" || this.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        /// <summary>
        /// 是否为event-stream请求
        /// </summary>
        /// <returns></returns>
        public bool IsEventStreamRequest()
        {
            return StringEquals(this.Headers["Accept"], "text/event-stream");
        }

        /// <summary>
        /// Content-Type是否为
        /// application/x-www-form-urlencoded
        /// </summary>
        /// <returns></returns>
        public bool IsApplicationFormRequest()
        {
            if (this.HttpMethod == HttpMethod.GET)
            {
                return false;
            }

            ContentType contentType = new ContentType(this);
            return contentType.IsMatch("application/x-www-form-urlencoded");
        }

        /// <summary>
        /// Content-Type是否为
        /// application/json
        /// </summary>
        /// <param name="charset">字符编码</param>
        /// <returns></returns>
        public bool IsRawJsonRequest(out Encoding charset)
        {
            charset = null;
            if (this.HttpMethod == HttpMethod.GET)
            {
                return false;
            }

            ContentType contentType = new ContentType(this);
            if (contentType.IsMatch("application/json") == false)
            {
                return false;
            }

            string encoding = contentType.TryGetExtend("chartset", "utf-8");
            charset = Encoding.GetEncoding(encoding);
            return true;
        }

        /// <summary>
        /// Content-Type是否为
        /// multipart/form-data
        /// </summary>
        /// <returns></returns>
        public bool IsMultipartFormRequest(out string boundary)
        {
            boundary = null;
            if (this.HttpMethod == HttpMethod.GET)
            {
                return false;
            }

            ContentType contentType = new ContentType(this);
            if (contentType.IsMatch("multipart/form-data") == false)
            {
                return false;
            }
            bool state = contentType.TryGetExtend("boundary", out boundary);
            if (state && boundary.StartsWith("\"") && boundary.EndsWith("\""))
            {
                boundary = boundary.Substring(1, boundary.Length - 2);
            }
            return state;
        }

        /// <summary>
        /// 获取是否为Websocket请求
        /// </summary>
        /// <returns></returns>
        public bool IsWebsocketRequest()
        {
            if (this.HttpMethod != Http.HttpMethod.GET)
            {
                return false;
            }
            if (StringContains(this.Headers.TryGet<string>("Connection"), "Upgrade") == false)
            {
                return false;
            }
            if (this.Headers.TryGet<string>("Upgrade") == null)
            {
                return false;
            }
            if (StringEquals(this.Headers.TryGet<string>("Sec-WebSocket-Version"), "13") == false)
            {
                return false;
            }
            if (this.Headers.TryGet<string>("Sec-WebSocket-Key") == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 返回客户端是否接受GZip压缩
        /// </summary>
        /// <returns></returns>
        public bool IsAcceptGZip()
        {
            var accept = this.Headers["Accept-Encoding"];
            if (accept == null)
            {
                return false;
            }
            return accept.IndexOf("gzip", StringComparison.OrdinalIgnoreCase) > -1;
        }

        /// <summary>
        /// 是否支持KeepAlive
        /// </summary>
        /// <returns></returns>
        public bool IsKeepAlive()
        {
            var connection = this.Headers["Connection"];
            return StringEquals(connection, "close") == false;
        }

        /// <summary>
        /// 获取是否相等
        /// 不区分大小写
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static bool StringEquals(string value1, string value2)
        {
            return string.Equals(value1, value2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取是是否包含
        /// 不区分大小写
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static bool StringContains(string value1, string value2)
        {
            if (string.IsNullOrEmpty(value1) || string.IsNullOrEmpty(value2))
            {
                return false;
            }
            return value1.IndexOf(value2, StringComparison.OrdinalIgnoreCase) > -1;
        }


    }
}
