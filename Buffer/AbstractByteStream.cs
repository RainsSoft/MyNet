using MyNet.Common;
using System;
using System.IO;
using System.Text;

namespace MyNet.Buffer
{
    public abstract class AbstractByteStream : BaseDisposable, IByteStream
    {
        protected int _readerindex = 0;
        protected int _writerindex = 0;
        public int ReaderIndex
        {
            get
            {
                return _readerindex;
            }
        }

        public int WriterIndex
        {
            get
            {
                return _writerindex;
            }
        }

        public int Length
        {
            get { return _writerindex; }
        }

        public void SetReaderIndex(int index)
        {
            if (index < 0 || index > _writerindex)
            {
                AgentLogger.Instance.Err("读超出索引范围");
                return;
            }
            _readerindex = index;
        }

        public void SetWriterIndex(int index)
        {
            if (index < 0 || index >= Capacity)
            {
                AgentLogger.Instance.Err("写超出索引范围");
                return;
            }
            _writerindex = index;
        }
        public bool ReadSkip(int len)
        {
            int newindex = _readerindex + len;
            if (newindex <= _writerindex)
            {
                _readerindex = newindex;
                return true;
            }
            return false;
        }
        public string ReadString(Encoding coding, int len)
        {
            byte[] data = ReadBytes(len);
            return coding.GetString(data);
        }
        public byte ReadByte()
        {
            byte rt = GetByte(_readerindex);
            _readerindex += 1;
            return rt;
        }
        public byte[] ReadBytes(int len)
        {
            byte[] rt = GetBytes(_readerindex, len);
            _readerindex += len;
            return rt;
        }
        public byte[] ToArray()
        {
            return GetBytes(0, Length);
        }
        public void WriteByte(byte b)
        {
            EnsureWritable(1);
            SetByte(_writerindex, b);
            _writerindex += 1;
        }
        public void WriteBytes(byte[] bs)
        {
            EnsureWritable(bs.Length);
            SetBytes(_writerindex, bs);
            _writerindex += bs.Length;
        }
        public void WriteBytes(byte[] bs, int offset, int len)
        {
            EnsureWritable(len);
            SetBytes(_writerindex, bs, offset, len);
            _writerindex += len;
        }
        public void SetBytes(int start, byte[] bs)
        {
            SetBytes(start, bs, 0, bs.Length);
        }
        public bool StartWith(byte[] bs)
        {
            int startindex = _readerindex;
            int cj = 0;
            if (this.GetByte(startindex) == bs[cj])
            {
                for (cj = 1; cj < bs.Length; cj++)
                {
                    if (this.GetByte(startindex + cj) != bs[cj])
                    {
                        break;
                    }
                }
                if (cj == bs.Length)
                {
                    return true;
                }
            }
            return false;
        }
        public int IndexOf(int fromIndex, int toIndex, byte value)
        {
            int startindex = fromIndex;
            for (; startindex < toIndex; startindex++)
            {
                if (this.GetByte(startindex) == value)
                {
                    return startindex;
                }
            }
            return -1;
        }
        public int IndexOf(byte b)
        {
            return IndexOf(_readerindex, Length, b);
        }
        public int IndexOf(int fromIndex, int toIndex, byte[] value)
        {
            int startindex = fromIndex;
            for (; startindex < toIndex; startindex++)
            {
                int cj = 0;
                if (this.GetByte(startindex) == value[cj])
                {
                    for (cj = 1; cj < value.Length; cj++)
                    {
                        if (this.GetByte(startindex + cj) != value[cj])
                        {
                            break;
                        }
                    }
                    if (cj == value.Length)
                    {
                        return startindex;
                    }
                }
            }
            return -1;
        }
        public int IndexOf(byte[] bs)
        {
            return IndexOf(_readerindex, Length, bs);
        }

        public IByteStream EnsureWritable(int newWriteBytes)
        {
            int writablebytes = this.Capacity - _writerindex;
            if (newWriteBytes <= writablebytes)
            {
                return this;
            }
            AdjustCapacity(_writerindex + newWriteBytes);
            return this;
        }
        public string ToString(Encoding coding)
        {
            byte[] data = GetBytes(0, Length);
            return coding.GetString(data);
        }
        public override string ToString()
        {
            return ToString(Encoding.UTF8);
        }
        public short GetShort(int index)
        {
            return (short)(this.GetByte(index) << 8 | this.GetByte(index + 1));
        }

        public int GetInt(int index)
        {
            return this.GetShort(index) << 16 | (ushort)this.GetShort(index + 2);
        }

        public long GetLong(int index)
        {
            return (long)this.GetInt(index) << 32 | (uint)this.GetInt(index + 4);
        }

        public abstract int Capacity { get; }
        public abstract byte GetByte(int index);
        public abstract byte[] GetBytes(int start, int len);
        public abstract IByteStream AdjustCapacity(int newCapacity);
        public abstract ArraySegment<byte> GetIOBuffer();

        public abstract void SetBytes(int start, byte[] bs, int offset, int len);
        public abstract void SetByte(int start, byte b);
        public abstract void Clear();

        public short ReadShort()
        {
            short rt = GetShort(_readerindex);
            _readerindex += 2;
            return rt;
        }

        public int ReadInt()
        {
            int rt = GetInt(_readerindex);
            _readerindex += 4;
            return rt;
        }

        public long ReadLong()
        {
            long rt = GetLong(_readerindex);
            _readerindex += 8;
            return rt;
        }
        public int BytesBefore(byte value)
        {
            return BytesBefore(_readerindex, _writerindex, value);
        }
        public int BytesBefore(int length, byte value)
        {
            return BytesBefore(_readerindex, length, value);
        }
        public int BytesBefore(int index, int length, byte value)
        {
            int endIndex = IndexOf(index, index + length, value);
            //如果小于零，代表没找到
            if (endIndex < 0)
            {
                return -1;
            }
            //返回value的位置索引减去起始索引
            return endIndex - index;
        }

        public bool IsReadable()
        {
            return _writerindex > _readerindex;
        }

        public abstract IByteStream Slice(int start, int len);
        public abstract IByteStream ToBase64();

        public int BytesBefore(byte[] value)
        {
            return BytesBefore(_readerindex, _writerindex, value);
        }

        public int BytesBefore(int length, byte[] value)
        {
            return BytesBefore(_readerindex, length, value);
        }

        public int BytesBefore(int index, int length, byte[] value)
        {
            int endIndex = IndexOf(index, index + length, value);
            //如果小于零，代表没找到
            if (endIndex < 0)
            {
                return -1;
            }
            //返回value的位置索引减去起始索引
            return endIndex - index;
        }

        public void WriteLong(long value)
        {
            EnsureWritable(8);
            SetByte(_writerindex, (byte)(value >> 56));
            SetByte(_writerindex + 1, (byte)(value >> 48));
            SetByte(_writerindex + 2, (byte)(value >> 40));
            SetByte(_writerindex + 3, (byte)(value >> 32));
            SetByte(_writerindex + 4, (byte)(value >> 24));
            SetByte(_writerindex + 5, (byte)(value >> 16));
            SetByte(_writerindex + 6, (byte)(value >> 8));
            SetByte(_writerindex + 7, (byte)(value));
            _writerindex += 8;
        }

        public void WriteInt(int value)
        {
            EnsureWritable(4);
            SetByte(_writerindex, (byte)(value >> 24));
            SetByte(_writerindex + 1, (byte)(value >> 16));
            SetByte(_writerindex + 2, (byte)(value >> 8));
            SetByte(_writerindex + 3, (byte)(value));
            _writerindex += 4;
        }

        public void WriteShort(short value)
        {
            EnsureWritable(2);
            SetByte(_writerindex, (byte)(value >> 8));
            SetByte(_writerindex + 1, (byte)(value));
            _writerindex += 2;

        }


        public short ReadLEShort()
        {
            short rt = GetLEShort(_readerindex);
            _readerindex += 2;
            return rt;
        }

        public int ReadLEInt()
        {
            int rt = GetLEInt(_readerindex);
            _readerindex += 4;
            return rt;
        }

        public long ReadLELong()
        {
            long rt = GetLELong(_readerindex);
            _readerindex += 8;
            return rt;
        }

        public short GetLEShort(int index)
        {
            return (short)(this.GetByte(index + 1) << 8 | this.GetByte(index));
        }

        public int GetLEInt(int index)
        {
            return this.GetShort(index + 2) << 16 | (ushort)this.GetShort(index);
        }

        public long GetLELong(int index)
        {
            return (long)this.GetInt(index + 4) << 32 | (uint)this.GetInt(index);
        }

        public void WriteLELong(long value)
        {
            EnsureWritable(8);
            SetByte(_writerindex, (byte)(value));
            SetByte(_writerindex + 1, (byte)(value >> 8));
            SetByte(_writerindex + 2, (byte)(value >> 16));
            SetByte(_writerindex + 3, (byte)(value >> 24));
            SetByte(_writerindex + 4, (byte)(value >> 32));
            SetByte(_writerindex + 5, (byte)(value >> 40));
            SetByte(_writerindex + 6, (byte)(value >> 48));
            SetByte(_writerindex + 7, (byte)(value >> 56));
            _writerindex += 8;
        }

        public void WriteLEInt(int value)
        {
            EnsureWritable(4);
            SetByte(_writerindex, (byte)(value));
            SetByte(_writerindex + 1, (byte)(value >> 8));
            SetByte(_writerindex + 2, (byte)(value >> 16));
            SetByte(_writerindex + 3, (byte)(value >> 24));
            _writerindex += 4;
        }

        public void WriteLEShort(short value)
        {
            EnsureWritable(2);
            SetByte(_writerindex, (byte)(value));
            SetByte(_writerindex + 1, (byte)(value >> 8));
            _writerindex += 2;
        }
    }
}
