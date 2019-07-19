using System;
using System.IO;
using System.Text;

namespace MyNet.Buffer
{
    public interface IByteStream : IDisposable
    {
        /// <summary>
        /// 当前读索引
        /// </summary>
        int ReaderIndex { get; }
        /// <summary>
        /// 当前写索引
        /// </summary>
        int WriterIndex { get; }
        /// <summary>
        /// 容量
        /// </summary>
        int Capacity { get; }
        /// <summary>
        /// 实际使用长度
        /// </summary>
        int Length { get; }
        void SetReaderIndex(int index);
        void SetWriterIndex(int index);
        int IndexOf(int fromIndex, int toIndex, byte value);
        /// <summary>
        /// 索引为从当前读取位置开始
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        int IndexOf(byte b);
        int IndexOf(int fromIndex, int toIndex, byte[] value);
        /// <summary>
        /// 索引为从当前读取位置开始
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        int IndexOf(byte[] bs);
        bool StartWith(byte[] bs);
        bool ReadSkip(int len);
        byte ReadByte();
        byte[] ReadBytes(int len);
        short ReadShort();
        short ReadLEShort();
        int ReadInt();
        int ReadLEInt();
        long ReadLong();
        long ReadLELong();
        string ReadString(Encoding coding, int len);
        string ToString(Encoding coding);
        short GetShort(int index);
        short GetLEShort(int index);
        int GetInt(int index);
        int GetLEInt(int index);
        long GetLong(int index);
        long GetLELong(int index);
        byte GetByte(int index);
        byte[] GetBytes(int start, int len);
        void SetByte(int start, byte b);
        void SetBytes(int start, byte[] bs);
        void SetBytes(int start, byte[] bs, int offset, int len);
        void WriteByte(byte b);
        void WriteBytes(byte[] bs);
        void WriteBytes(byte[] bs, int offset, int len);
        void WriteLong(long value);
        void WriteLELong(long value);
        void WriteInt(int value);
        void WriteLEInt(int value);
        void WriteShort(short value);
        void WriteLEShort(short value);
        byte[] ToArray();
        /// <summary>
        /// 清零数据
        /// </summary>
        void Clear();
        /// <summary>
        /// 确保写入字节不超过
        /// </summary>
        /// <param name="newWriteBytes"></param>
        /// <returns></returns>
        IByteStream EnsureWritable(int newWriteBytes);
        /// <summary>
        /// 校正容量
        /// </summary>
        /// <param name="newCapacity"></param>
        /// <returns></returns>
        IByteStream AdjustCapacity(int newCapacity);

        ArraySegment<byte> GetIOBuffer();
        int BytesBefore(byte value);
        int BytesBefore(int length, byte value);
        int BytesBefore(int index, int length, byte value);
        int BytesBefore(byte[] value);
        int BytesBefore(int length, byte[] value);
        int BytesBefore(int index, int length, byte[] value);
        bool IsReadable();
        IByteStream ToBase64();
        IByteStream Slice(int start, int len);
    }
}
