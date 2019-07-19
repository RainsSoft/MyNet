using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Common;
using MyNet.Handlers;
using System;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;


namespace MyNet.Middleware.SSL
{

    /// <summary>
    /// SSL代理验证、封包、解包
    /// </summary>
    public class SSLAgentStream : Stream
    {
        byte[] _input;
        int _inputStartOffset;
        int _inputOffset;
        int _inputLength;
        IContext _context;
        SslStream _sslstream;
        X509Certificate _cert;
        SSLAsyncResult<int> _AsyncRead;
        SSLAsyncResult<int> _AsyncWrite;
        SSLHandlerState _state = SSLHandlerState.UnAuthentication;
        WritePacket _curwritePacket = null;
        bool _needRead = false;
        public SSLHandlerState State
        {
            get { return _state; }
            set { _state = value; }
        }
        public bool NeedRead
        {
            get { return _needRead; }
            set { _needRead = value; }
        }
        public SSLAgentStream(IContext context, X509Certificate cert)
        {
            _cert = cert;
            _context = context;
            _sslstream = new SslStream(this, false);
        }
        public void BeginAuthenticateAsServer(AsyncCallback asyncCallback)
        {
            _needRead = true;
            _sslstream.BeginAuthenticateAsServer(_cert, false, SslProtocols.Default, false, asyncCallback, _sslstream);
        }
        /// <summary>
        /// 握手阶段触发读取数据
        /// </summary>
        public void TriggerHandshakeRead()
        {
            if (_needRead)
            {
                _needRead = false;
                if (_AsyncWrite != null)
                {
                    _AsyncWrite.Finish();
                }
            }
            else
            {
                if (_AsyncRead != null)
                {
                    ArraySegment<byte> sslBuffer = _AsyncRead.SSLAsyncBuffer;
                    int read = this.ReadFromInput(sslBuffer.Array, sslBuffer.Offset, sslBuffer.Count);
                    _AsyncRead.Result = read;
                    _AsyncRead.Finish();
                }
            }

        }

        public void SetSource(byte[] source, int offset, int len)
        {
            this._input = source;
            this._inputStartOffset = offset;
            this._inputOffset = 0;
            this._inputLength = len;
        }
        public void WriteToSslStream(WritePacket packet)
        {
            _curwritePacket = packet;
            ArraySegment<byte> tmparr = packet.Stream.GetIOBuffer();
            this._sslstream.Write(tmparr.Array, tmparr.Offset, packet.Stream.Length);
        }
        public void ReadFromSslStream(IByteStream outstream)
        {
            byte[] tmpreads = new byte[this._inputLength];
            int readsize = -1;
            do
            {
                readsize = this._sslstream.Read(tmpreads, 0, this._inputLength);
                outstream.WriteBytes(tmpreads, 0, readsize);
            } while (readsize != 0);
        }
        IAsyncResult PrepareSyncReadResult(int readBytes, object state)
        {
            SSLSyncResult<int> result = new SSLSyncResult<int>();
            result.Result = readBytes;
            result.AsyncState = state;
            return result;
        }
        int ReadFromInput(byte[] destination, int destinationOffset, int destinationCapacity)
        {
            byte[] source = this._input;
            int readableBytes = this._inputLength - this._inputOffset;
            int length = Math.Min(readableBytes, destinationCapacity);
            System.Buffer.BlockCopy(source, this._inputStartOffset + this._inputOffset, destination, destinationOffset, length);
            this._inputOffset += length;
            return length;
        }
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (this._inputLength - this._inputOffset > 0)
            {
                //有数据则直接读取
                int read = this.ReadFromInput(buffer, offset, count);
                return this.PrepareSyncReadResult(read, state);
            }
            _AsyncRead = new SSLAsyncResult<int>();
            _AsyncRead.SSLAsyncBuffer = new ArraySegment<byte>(buffer, offset, count);
            _AsyncRead.Callback = callback;
            _AsyncRead.AsyncState = state;
            return _AsyncRead;
        }
        public override int EndRead(IAsyncResult asyncResult)
        {
            SSLResult<int> result = asyncResult as SSLResult<int>;
            if (_AsyncRead != null)
            {
                _AsyncRead.SSLAsyncBuffer = default(ArraySegment<byte>);
                _AsyncRead.Callback = null;
                _AsyncRead = null;
            }
            return result.Result;
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.ReadFromInput(buffer, offset, count);
        }
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            _curwritePacket = null;
            Write(buffer, offset, count);
            if ((_state & SSLHandlerState.Authenticating) != 0 && _needRead)
            {
                //如果是在握手阶段则使用异步,需要读取才结束
                _AsyncWrite = new SSLAsyncResult<int>();
                _AsyncWrite.Callback = callback;
                _AsyncWrite.AsyncState = state;
                return _AsyncWrite;
            }
            else
            {
                SSLSyncResult<int> result = new SSLSyncResult<int>();
                result.AsyncState = state;
                return result;
            }
        }
        public override void EndWrite(IAsyncResult asyncResult)
        {
            return;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            SSLWritePacket wp = new SSLWritePacket(_curwritePacket);
            wp.Stream.WriteBytes(buffer, offset, count);
            _context.Channel.SendAsync(wp);
        }

        public override bool CanSeek
        {
            get
            {
                return false;
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
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }


    }
}
