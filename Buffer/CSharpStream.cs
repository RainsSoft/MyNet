using System;
using System.IO;

namespace MyNet.Buffer
{
    /// <summary>
    /// 将ByteStream转成C#流
    /// </summary>
    public class CSharpStream : Stream
    {
        IByteStream _bytestream;
        long _position;
        public CSharpStream(IByteStream bytestream)
        {
            _bytestream = bytestream;
            _position = 0;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return _bytestream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int readlen = count > (int)(_bytestream.Length - _position) ? (int)(_bytestream.Length - _position) : count;
            if (readlen > 0)
            {
                ArraySegment<byte> buff = _bytestream.GetIOBuffer();
                System.Buffer.BlockCopy(buff.Array
                    , (int)_position + buff.Offset
                    , buffer, offset
                    , readlen);
                _position += readlen;
                return readlen;
            }
            else
            {
                return 0;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long oldValue = _position;
            switch ((int)origin)
            {
                case (int)SeekOrigin.Begin:
                    _position = offset;
                    break;
                case (int)SeekOrigin.End:
                    _position = _bytestream.Length - offset;
                    break;
                case (int)SeekOrigin.Current:
                    _position += offset;
                    break;
                default:
                    throw new InvalidOperationException("未知的 SeekOrigin 类型");
            }
            if (_position < 0 || _position > _bytestream.Length)
            {
                _position = oldValue;
                throw new IndexOutOfRangeException();
            }
            return _position;
        }

        public override void SetLength(long value)
        {
            if (value > int.MaxValue)
            {
                throw new IndexOutOfRangeException("CSharpStream溢出");
            }
            if (value < 0)
            {
                throw new IndexOutOfRangeException("CSharpStream下溢");
            }
            int newlen = (int)value;
            int newwritelen = newlen - _bytestream.Length;
            if (newwritelen > 0)
            {
                _bytestream.EnsureWritable(newwritelen);
                _bytestream.SetWriterIndex(newlen);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int cur = (int)_position;
            int endOffset = cur + count;
            int newbytes = endOffset - _bytestream.Length;
            if (newbytes > 0)
            {
                _bytestream.EnsureWritable(newbytes);
                _bytestream.SetWriterIndex(_bytestream.Length + newbytes);
            }
            _bytestream.SetBytes(cur, buffer, offset, count);
            _position = endOffset;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_bytestream != null)
            {
                _bytestream.Dispose();
            }
        }

    }
}
