using System;

namespace MyNet.Common
{
    public struct ByteBits : IComparable<ByteBits>
    {
        /// <summary>
        /// 字节值
        /// </summary>
        private byte value;

        /// <summary>
        /// 右移运算
        /// </summary>
        /// <param name="count">移动位数</param>
        /// <returns></returns>
        public ByteBits MoveRight(int count)
        {
            return (byte)(this.value >> count);
        }

        /// <summary>
        /// 左移运算
        /// </summary>
        /// <param name="count">移动位数</param>
        /// <returns></returns>
        public ByteBits MoveLeft(int count)
        {
            return (byte)(this.value << count);
        }

        /// <summary>
        /// 取高位
        /// 相当于右移8-count个单位
        /// </summary>
        /// <param name="count">位的数量</param>
        /// <returns></returns>
        public ByteBits Take(int count)
        {
            return this.MoveRight(8 - count);
        }

        /// <summary>
        /// 从指定索引位置取高位
        /// 相当于先左移index个单位再右移8-count个单位
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="count">位的数量</param>
        /// <returns></returns>
        public ByteBits Take(int index, int count)
        {
            return this.MoveLeft(index).MoveRight(8 - count);
        }


        /// <summary>
        /// byte的位集合
        /// </summary>
        /// <param name="value">字节</param>
        private ByteBits(byte value)
        {
            this.value = value;
        }

        /// <summary>
        /// 获取或设置指定位的值
        /// </summary>
        /// <param name="index">由高到低的位索引</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public bool this[int index]
        {
            get
            {
                if (index < 0 || index > 7)
                {
                    throw new ArgumentOutOfRangeException("index", "index为0到7之间");
                }
                return ((this.value & (128 >> index)) > 0) ? true : false;
            }
            set
            {
                if (index < 0 || index > 7)
                {
                    throw new ArgumentOutOfRangeException("index", "index为0到7之间");
                }

                if (value == true)
                {
                    this.value = (byte)(this.value | (128 >> index));
                }
                else
                {
                    this.value = (byte)(this.value & (byte.MaxValue - (128 >> index)));
                }
            }
        }

        /// <summary>
        /// 字符串显示各个位的值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            ByteBits bits = this;
            string[] _sarr = new string[8];
            for(int index = 0; index < 8; index++)
            {
                _sarr[index] = bits[index] ? "1" : "0";
            }
            return string.Join(", ", _sarr);
        }

        /// <summary>
        /// 从byte类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator ByteBits(byte value)
        {
            return new ByteBits(value);
        }

        /// <summary>
        /// 隐式转换为byte类型
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static implicit operator byte(ByteBits bits)
        {
            return bits.value;
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        /// <summary>
        /// 比较是否和目标相等
        /// </summary>
        /// <param name="obj">目标</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return (obj is ByteBits) && obj.GetHashCode() == this.GetHashCode();
        }


        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="other">目标</param>
        /// <returns></returns>
        int IComparable<ByteBits>.CompareTo(ByteBits other)
        {
            if (this == other)
            {
                return 0;
            }

            if (this.value > other.value)
            {
                return 1;
            }

            return -1;
        }
    }
}
