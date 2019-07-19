using MyNet.Common.Converts;
using System;
using System.Collections.Generic;

namespace MyNet.Common
{
    /// <summary>
    /// 提供丰富的类型转换功能
    /// </summary>
    public sealed class Converter
    {
        /// <summary>
        /// 转换器静态实例
        /// </summary>
        private static readonly Converter Instance = new Converter();

        /// <summary>       
        /// 支持基础类型、decimal、guid和枚举相互转换以及这些类型的可空类型和数组类型相互转换
        /// 支持字典转换为对象以及字典的数组转换为对象数组
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value">值</param>      
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public static T Cast<T>(object value)
        {
            return Converter.Instance.Convert<T>(value);
        }

        /// <summary>
        /// 支持基础类型、decimal、guid和枚举相互转换以及这些类型的可空类型和数组类型相互转换
        /// 支持字典转换为对象以及字典的数组转换为对象数组
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>       
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public static object Cast(object value, Type targetType)
        {
            return Converter.Instance.Convert(value, targetType);
        }
        /// <summary>
        /// 返回由字节数组中指定位置的四个字节转换来的16位有符号整数
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="startIndex">位置</param>   
        public static short ToInt16(byte[] bytes, int startIndex)
        {
            return (short)(bytes[startIndex] << 8 | bytes[1 + startIndex]);
        }

        public static short ToLEInt16(byte[] bytes, int startIndex)
        {
            return (short)(bytes[1 + startIndex] << 8 | bytes[startIndex]);
        }

        public static int ToInt(byte[] bytes, int startIndex)
        {
            return ToInt16(bytes, startIndex) << 16 | (ushort)ToInt16(bytes, startIndex + 2);
        }
        public static int ToLEInt(byte[] bytes, int startIndex)
        {
            return ToInt16(bytes, startIndex + 2) << 16 | (ushort)ToInt16(bytes, startIndex);
        }

        public static long ToLong(byte[] bytes, int startIndex)
        {
            return (long)ToInt(bytes, startIndex) << 32 | (uint)ToInt(bytes, startIndex + 4);
        }
        public static long ToLELong(byte[] bytes, int startIndex)
        {
            return (long)ToInt(bytes, startIndex + 4) << 32 | (uint)ToInt(bytes, startIndex);
        }
        /// <summary>
        /// 返回由64位有符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>    
        /// <returns></returns>
        public static byte[] ToBytes(long value)
        {
            byte[] bytes = new byte[8];
            bytes[7] = (byte)(value);
            bytes[6] = (byte)(value >> 8);
            bytes[5] = (byte)(value >> 16);
            bytes[4] = (byte)(value >> 24);
            bytes[3] = (byte)(value >> 32);
            bytes[2] = (byte)(value >> 40);
            bytes[1] = (byte)(value >> 48);
            bytes[0] = (byte)(value >> 56);
            return bytes;
        }
        public static byte[] ToLEBytes(long value)
        {
            byte[] bytes = new byte[8];
            bytes[0] = (byte)(value);
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);
            bytes[4] = (byte)(value >> 32);
            bytes[5] = (byte)(value >> 40);
            bytes[6] = (byte)(value >> 48);
            bytes[7] = (byte)(value >> 56);
            return bytes;
        }

    
        /// <summary>
        /// 返回由32位有符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>    
        /// <returns></returns>
        public static byte[] ToBytes(int value)
        {
            byte[] bytes = new byte[4];
            bytes[3] = (byte)(value);
            bytes[2] = (byte)(value >> 8);
            bytes[1] = (byte)(value >> 16);
            bytes[0] = (byte)(value >> 24);
            return bytes;
        }
        public static byte[] ToLEBytes(int value)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(value);
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);
            return bytes;
        }
      

        /// <summary>
        /// 返回由16位有符号整数转换为的字节数组
        /// </summary>
        /// <param name="value">整数</param>    
        /// <param name="isLittle">高低位</param>
        /// <returns></returns>
        public static byte[] ToBytes(short value)
        {
            byte[] bytes = new byte[2];
            bytes[1] = (byte)(value);
            bytes[0] = (byte)(value >> 8);
            return bytes;
        }
        public static byte[] ToBytes(ushort value)
        {
            byte[] bytes = new byte[2];
            bytes[1] = (byte)(value);
            bytes[0] = (byte)(value >> 8);
            return bytes;
        }
        public static byte[] ToLEBytes(short value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(value);
            bytes[1] = (byte)(value >> 8);
            return bytes;
        }
        public static byte[] ToLEBytes(ushort value)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(value);
            bytes[1] = (byte)(value >> 8);
            return bytes;
        }
        /// <summary>
        /// 获取转换单元操控对象        
        /// </summary>
        public ContertItems Items { get; private set; }

        /// <summary>
        /// 类型转换
        /// </summary>
        public Converter()
        {
            this.Items = new ContertItems(this)
                .AddLast<NoConvert>()
                .AddLast<NullConvert>()
                .AddLast<TimeConvert>()
                .AddLast<SimpleContert>()
                .AddLast<NullableConvert>()
                .AddLast<DictionaryConvert>()
                .AddLast<ArrayConvert>()
                .AddLast<ListConvert>();
        }

        /// <summary>
        /// 转换为目标类型
        /// </summary>
        /// <typeparam name="T">要转换的目标类型</typeparam>
        /// <param name="value">要转换的值</param>    
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>转换后的值</returns>
        public T Convert<T>(object value)
        {
            return (T)this.Convert(value, typeof(T));
        }

        /// <summary>
        /// 转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">要转换的目标类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>转换后的值</returns>
        public object Convert(object value, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            return this.Items.First.Convert(value, targetType);
        }


        /// <summary>
        /// 表示转换器的转换单元合集
        /// </summary>
        public class ContertItems
        {
            /// <summary>
            /// 转换器实例
            /// </summary>
            private readonly Converter converter;

            /// <summary>
            /// 转换单元列表
            /// </summary>           
            private readonly LinkedList<IConvert> linkedList = new LinkedList<IConvert>();

            /// <summary>
            /// 获取第一个转换单元
            /// </summary>
            internal IConvert First
            {
                get
                {
                    return this.linkedList.First.Value;
                }
            }

            /// <summary>
            /// 转换单元合集
            /// </summary>
            /// <param name="converter">转换器实例</param>
            public ContertItems(Converter converter)
            {
                this.converter = converter;
                this.linkedList.AddLast(new NotSupportedConvert());
            }

            /// <summary>
            /// 通过类型查找节点
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <returns></returns>
            private LinkedListNode<IConvert> FindNode<T>() where T : IConvert
            {
                LinkedListNode<IConvert> node = this.linkedList.First;
                while (node != null)
                {
                    if (node.Value.GetType() == typeof(T))
                    {
                        return node;
                    }
                    node = node.Next;
                }
                return null;
            }

            /// <summary>
            /// 是否已存在T类型的转换单元
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            private bool ExistConvert<T>() where T : IConvert
            {
                return this.FindNode<T>() != null;
            }

            /// <summary>
            /// 初始化各转换单元
            /// </summary>
            /// <returns></returns>
            private ContertItems ReInitItems()
            {
                LinkedListNode<IConvert> node = this.linkedList.First;
                while (node.Next != null)
                {
                    node.Value.NextConvert = node.Next.Value;
                    node.Value.Converter = this.converter;
                    node = node.Next;
                }
                return this;
            }

            /// <summary>
            /// 添加一个转换单元到最前面
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems AddFrist<T>() where T : IConvert
            {
                if (this.ExistConvert<T>() == false)
                {
                    T convert = Activator.CreateInstance<T>();
                    this.linkedList.AddFirst(convert);
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 添加到指定转换单元之后
            /// </summary>
            /// <typeparam name="TSource">已存在的转换单元</typeparam>
            /// <typeparam name="TDest">新加入的转换单元</typeparam>
            /// <returns></returns>
            public ContertItems AddBefore<TSource, TDest>()
                where TSource : IConvert
                where TDest : IConvert
            {
                LinkedListNode<IConvert> node = this.FindNode<TSource>();
                if (node != null && this.ExistConvert<TDest>() == false)
                {
                    TDest convert = Activator.CreateInstance<TDest>();
                    this.linkedList.AddBefore(node, convert);
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 添加到指定转换单元之后
            /// </summary>
            /// <typeparam name="TSource">已存在的转换单元</typeparam>
            /// <typeparam name="TDest">新加入的转换单元</typeparam>
            /// <returns></returns>
            public ContertItems AddAfter<TSource, TDest>()
                where TSource : IConvert
                where TDest : IConvert
            {
                LinkedListNode<IConvert> node = this.FindNode<TSource>();
                if (node != null && this.ExistConvert<TDest>() == false)
                {
                    TDest convert = Activator.CreateInstance<TDest>();
                    this.linkedList.AddAfter(node, convert);
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 添加一个转换单元到末尾
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems AddLast<T>() where T : IConvert
            {
                return this.AddBefore<NotSupportedConvert, T>();
            }

            /// <summary>
            /// 解绑一个转换单元
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems Remove<T>() where T : IConvert
            {
                LinkedListNode<IConvert> node = this.FindNode<T>();
                if (node != null)
                {
                    this.linkedList.Remove(node);
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 替换转换单元
            /// </summary>
            /// <typeparam name="TSource">要被替换掉的转换单元</typeparam>
            /// <typeparam name="TDest">替换后的转换单元</typeparam>
            /// <returns></returns>
            public ContertItems Repace<TSource, TDest>()
                where TSource : IConvert
                where TDest : IConvert
            {
                LinkedListNode<IConvert> node = this.FindNode<TSource>();
                if (node != null && this.ExistConvert<TDest>() == false)
                {
                    TDest convert = Activator.CreateInstance<TDest>();
                    node.Value = convert;
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 清除所有转换单元
            /// </summary>
            public void Clear()
            {
                this.linkedList.Clear();
                this.linkedList.AddLast(new NotSupportedConvert());
            }


        }
    }
}
