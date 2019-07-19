using System;
using System.Collections;

namespace MyNet.Common.Converts
{
    /// <summary>
    /// 表示数组转换单元
    /// </summary>
    public class ArrayConvert : IConvert
    {
        /// <summary>
        /// 转换器实例
        /// </summary>
        public Converter Converter { get; set; }

        /// <summary>
        /// 下一个转换单元
        /// </summary>
        public IConvert NextConvert { get; set; }

        /// <summary>
        /// 将value转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">转换的目标类型</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType)
        {
            if (targetType.IsArray == false)
            {
                return this.NextConvert.Convert(value, targetType);
            }

            IEnumerable items = value as IEnumerable;
            Type elementType = targetType.GetElementType();

            if (items == null)
            {
                return Array.CreateInstance(elementType, 0);
            }

            var length = 0;
            IList list = items as IList;
            if (list != null)
            {
                length = list.Count;
            }
            else
            {
                IEnumerator enumerator = items.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    length = length + 1;
                }
            }

            int index = 0;
            Array array = Array.CreateInstance(elementType, length);
            foreach (object item in items)
            {
                object itemCast = this.Converter.Convert(item, elementType);
                array.SetValue(itemCast, index);
                index = index + 1;
            }
            return array;
        }
    }
}
