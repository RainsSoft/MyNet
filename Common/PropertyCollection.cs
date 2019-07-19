using System;
using System.Collections.Generic;

namespace MyNet.Common
{
    public class PropertyCollection
    {
        private Dictionary<string, object> _dict = new Dictionary<string, object>();
        public void TryAdd(string key, object value)
        {
            try
            {
                _dict[key] = value;
            }
            catch (Exception ex)
            {
                AgentLogger.Instance.Err(ex.Message);
            }
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
            try
            {
                object value;
                if (_dict.TryGetValue(key, out value))
                {
                    return Converter.Cast<T>(value);
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
