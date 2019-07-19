using MyNet.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MyNet.Middleware.Http
{
    public class HttpCookieCollection : IEnumerable
    {
        protected Dictionary<string, HttpCookie> _cookies = new Dictionary<string, HttpCookie>();
        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<string, HttpCookie> kv in _cookies)
            {
                yield return kv.Value;
            }
        }
        public HttpCookie this[string name]
        {
            get
            {
                HttpCookie cookie;
                if (_cookies.TryGetValue(name,out cookie))
                {
                    return cookie;
                }
                else
                {
                    return null;
                }
            }
        }
        public bool ExistCookie(string name)
        {
            return _cookies.ContainsKey(name);
        }
        public void Clear()
        {
            _cookies.Clear();
        }
       
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="cookie"></param>
        public void SetCookie(HttpCookie cookie)
        {
            if (cookie == null)
            {
                return;
            }
            _cookies[cookie.Name] = cookie;
        }
        /// <summary>
        /// 添加并对Cookie值进行Url编码
        /// </summary>
        /// <param name="cookie"></param>
        public void SetUrlCookie(HttpCookie cookie)
        {
            cookie.Value = Utility.UrlEncode(cookie.Value, Encoding.UTF8);
            SetCookie(cookie);
        }
    }
}
