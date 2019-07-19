using MyNet.Common;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MyNet.Middleware.Http
{
    /// <summary>
    /// 表示可通过键或索引访问的集合
    /// </summary>
    public class HttpNameValueCollection : NameValueCollection
    {
        /// <summary>
        /// 表示可通过键或索引访问的集合
        /// </summary>
        public HttpNameValueCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// 返回是否包含键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return this.AllKeys.Any(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 从参数字符串生成键或索引集合
        /// </summary>
        /// <param name="parameters">http请求原始参数</param>
        /// <returns></returns>
        public static HttpNameValueCollection Parse(string parameters)
        {
            HttpNameValueCollection collection = new HttpNameValueCollection();
            if (string.IsNullOrEmpty(parameters))
            {
                return collection;
            }

            string[] keyValues = parameters.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in keyValues)
            {
                string[] kv = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (kv.Length > 1)
                {
                    string key = Utility.UrlDecode(kv[0], Encoding.UTF8);
                    string value = kv.Length == 2 ? Utility.UrlDecode(kv[1], Encoding.UTF8) : null;
                    collection.Add(key, value);
                }
            }
            return collection;
        }
        /// <summary>
        /// 获取指定键的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T TryGet<T>(string key)
        {
            return this.TryGet<T>(key, default(T));
        }

        /// <summary>
        /// 获取指定键的值
        /// 失败则返回defaultValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public T TryGet<T>(string key, T defaultValue)
        {
            var value = this[key];
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            try
            {
                return Converter.Cast<T>(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
