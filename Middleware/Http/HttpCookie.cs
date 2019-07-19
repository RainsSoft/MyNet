using System;
namespace MyNet.Middleware.Http
{
    /// <summary>
    /// 正常cookie是没有子键的
    /// 微软的cookie自己设置了子键格式
    /// </summary>
    public class HttpCookie
    {
        public HttpCookie(string name) : this(name, "")
        {
        }

        public HttpCookie(string name, string value)
        {
            Expires = -1;
            Domain = "/";
            HttpOnly = true;
            Secure = false;
            Name = name;
            Value = value;
        }

        public string Domain { get; set; }
        /// <summary>
        /// -1则会话结束过期,值为Java的Ticks值
        /// </summary>
        public long Expires { get; set; }

        public bool HttpOnly { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public bool Secure { get; set; }
        public string Value { get; set; }
    }
}
