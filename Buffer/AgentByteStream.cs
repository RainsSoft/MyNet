using MyNet.Common;
using System;
using System.Text;

namespace MyNet.Buffer
{
    public class AgentByteStream : BaseDisposable, IByteStream
    {
        IByteStream _stream;
        public AgentByteStream(IByteStream stream)
        {
            _stream = stream;
        }
        public int Capacity
        {
            get
            {
                return _stream.Capacity;
            }
        }

        public int Length
        {
            get
            {
                return _stream.Length;
            }
        }

        public int ReaderIndex
        {
            get
            {
                return _stream.ReaderIndex;
            }
        }

        public int WriterIndex
        {
            get
            {
                return _stream.WriterIndex;
            }
        }

        public IByteStream AdjustCapacity(int newCapacity)
        {
            return _stream.AdjustCapacity(newCapacity);
        }

        public void Clear()
        {
            _stream.Clear();
        }

        public IByteStream EnsureWritable(int newWriteBytes)
        {
            return _stream.EnsureWritable(newWriteBytes);
        }

        public byte GetByte(int index)
        {
            return _stream.GetByte(index);
        }

        public byte[] GetBytes(int start, int len)
        {
            return _stream.GetBytes(start, len);
        }

        public int GetInt(int index)
        {
            return _stream.GetInt(index);
        }

        public ArraySegment<byte> GetIOBuffer()
        {
            return _stream.GetIOBuffer();
        }

        public short GetShort(int index)
        {
            return _stream.GetShort(index);
        }

        public int IndexOf(byte[] bs)
        {
            return _stream.IndexOf(bs);
        }

        public int IndexOf(byte b)
        {
            return _stream.IndexOf(b);
        }

        public byte[] ReadBytes(int len)
        {
            return _stream.ReadBytes(len);
        }

        public bool ReadSkip(int len)
        {
            return _stream.ReadSkip(len);
        }

        public string ReadString(Encoding coding, int len)
        {
            return _stream.ReadString(coding, len);
        }

        public void SetBytes(int start, byte[] bs)
        {
            _stream.SetBytes(start, bs);
        }

        public void SetBytes(int start, byte[] bs, int offset, int len)
        {
            _stream.SetBytes(start, bs, offset, len);
        }

        public void SetReaderIndex(int index)
        {
            _stream.SetReaderIndex(index);
        }

        public void SetWriterIndex(int index)
        {
            _stream.SetWriterIndex(index);
        }

        public bool StartWith(byte[] bs)
        {
            return _stream.StartWith(bs);
        }

        public byte[] ToArray()
        {
            return _stream.ToArray();
        }

        public string ToString(Encoding coding)
        {
            return _stream.ToString(coding);
        }

        public void WriteBytes(byte[] bs)
        {
            _stream.WriteBytes(bs);
        }

        public void WriteBytes(byte[] bs, int offset, int len)
        {
            _stream.WriteBytes(bs, offset, len);
        }

        protected override void OnUnManDisposed()
        {
            _stream.Dispose();
        }
        public override string ToString()
        {
            return _stream.ToString();
        }

        public byte ReadByte()
        {
            return _stream.ReadByte();
        }



        public long GetLong(int index)
        {
            return _stream.GetLong(index);
        }


        public short ReadShort()
        {
            return _stream.ReadShort();
        }

        public int ReadInt()
        {
            return _stream.ReadInt();
        }

        public long ReadLong()
        {
            return _stream.ReadLong();
        }

        public void SetByte(int start, byte b)
        {
            _stream.SetByte(start, b);
        }

        public void WriteByte(byte b)
        {
            _stream.WriteByte(b);
        }

        public int IndexOf(int fromIndex, int toIndex, byte value)
        {
            return _stream.IndexOf(fromIndex, toIndex, value);
        }

        public int IndexOf(int fromIndex, int toIndex, byte[] value)
        {
            return _stream.IndexOf(fromIndex, toIndex, value);
        }

        public int BytesBefore(int length, byte value)
        {
            return _stream.BytesBefore(length, value);
        }

        public int BytesBefore(int index, int length, byte value)
        {
            return _stream.BytesBefore(index, length, value);
        }

        public bool IsReadable()
        {
            return _stream.IsReadable();
        }
        public IByteStream Slice(int start, int len)
        {
            return _stream.Slice(start, len);
        }
        public IByteStream ToBase64()
        {
            return _stream.ToBase64();
        }

        public int BytesBefore(byte value)
        {
            return _stream.BytesBefore(value);
        }

        public int BytesBefore(byte[] value)
        {
            return _stream.BytesBefore(value);
        }

        public int BytesBefore(int length, byte[] value)
        {
            return _stream.BytesBefore(length, value);
        }

        public int BytesBefore(int index, int length, byte[] value)
        {
            return _stream.BytesBefore(index, length, value);
        }

        public void WriteLong(long value)
        {
            _stream.WriteLong(value);
        }

  

        public void WriteInt(int value)
        {
            _stream.WriteInt(value);
        }

        public void WriteShort(short value)
        {
            _stream.WriteShort(value);
        }


        public short ReadLEShort()
        {
            return _stream.ReadLEShort();
        }

        public int ReadLEInt()
        {
            return _stream.ReadLEInt();
        }

        public long ReadLELong()
        {
            return _stream.ReadLELong();
        }

        public short GetLEShort(int index)
        {
            return _stream.GetLEShort(index);
        }

        public int GetLEInt(int index)
        {
            return _stream.GetLEInt(index);
        }

        public long GetLELong(int index)
        {
            return _stream.GetLELong(index);
        }

        public void WriteLELong(long value)
        {
            _stream.WriteLELong(value);
        }

        public void WriteLEInt(int value)
        {
            _stream.WriteLEInt(value);
        }

        public void WriteLEShort(short value)
        {
            _stream.WriteLEShort(value);
        }
    }
}
