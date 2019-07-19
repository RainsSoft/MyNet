using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Handlers;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
namespace MyNet.Middleware.SSL
{
    public class SSLServerHandler : AbstractChannelHandler
    {
        private int _packetLength = 0;
        private SSLAgentStream _agent;

        /// <summary>
        /// 服务器证书
        /// </summary>
        public X509Certificate Certificate { get; private set; }

        public SSLServerHandler(string filename)
        {
            Certificate = new X509Certificate2(filename);
        }
        public SSLServerHandler(string filename, string password)
        {
            Certificate = new X509Certificate2(filename, password);
        }

        public SSLServerHandler(X509Certificate cert)
        {
            Certificate = cert;
        }

        public override void ChannelActive(IContext context)
        {
            ChannelBase channel = context.Channel;
            if (channel is TcpChannel)
            {
                _agent = new SSLAgentStream(context, Certificate);
            }

            context.FireNextActive();
        }

        public override void ChannelRead(IContext context, object msg)
        {
            IByteStream input = msg as IByteStream;
            if (input != null)
            {
                int endOffset = input.WriterIndex;

                // 使用该信息计算当前SSL记录的长度。
                if (this._packetLength > 0)
                {
                    if (endOffset < this._packetLength)
                    {
                        //数据未完整
                        context.Channel.MergeRead();
                        return;
                    }
                    else
                    {
                        this._packetLength = 0;
                    }
                }

                //判断是否为SSL加密包
                if (endOffset < SSLUtils.SSL_RECORD_HEADER_LENGTH)
                {
                    return;
                }

                int encryptedPacketLength = SSLUtils.GetEncryptedPacketLength(input, 0);
                if (encryptedPacketLength == -1)
                {
                    return;
                }

                if (encryptedPacketLength > endOffset)
                {
                    // 数据未完整
                    this._packetLength = encryptedPacketLength;
                    context.Channel.MergeRead();
                    return;
                }

                ArraySegment<byte> inputIoBuffer = input.GetIOBuffer();
                _agent.SetSource(inputIoBuffer.Array, inputIoBuffer.Offset, input.Length);
                if ((_agent.State & SSLHandlerState.Authenticating) != 0)
                {
                    _agent.TriggerHandshakeRead();
                }
                else
                {
                    if (EnsureAuthenticated())
                    {
                        IByteStream output = this.Unwrap(context, input);
                        context.FireNextRead(new SSLUnwrapStream(output));
                    }
                }


                return;
            }

            context.FireNextRead(msg);
        }

        public override void ChannelWrite(IContext context, object msg)
        {
            if (EnsureAuthenticated())
            {
                if (!(msg is SSLWritePacket))
                {
                    //判断是否非SSL包，是则进行SSL封包，并将当前包当作
                    WritePacket wp = msg as WritePacket;
                    if (wp != null)
                    {
                        _agent.WriteToSslStream(wp);
                        return;
                    }
                }

            }
            context.FirePreWrite(msg);
        }
        public override void ChannelWriteErr(IContext context, object msg)
        {
            SSLWritePacket sslpacket = msg as SSLWritePacket;
            if (sslpacket != null && sslpacket.Parent != null)
            {
                context.FireNextWriteErr(sslpacket.Parent);
            }
            else
            {
                context.FireNextWriteErr(msg);
            }
        }
        public override void ChannelWriteFinish(IContext context, object msg)
        {
            SSLWritePacket sslpacket = msg as SSLWritePacket;
            if (sslpacket != null && sslpacket.Parent != null)
            {
                context.FireNextWriteFinish(sslpacket.Parent);
            }
            else
            {
                context.FireNextWriteFinish(msg);
            }
        }
        /// <summary>
        /// SSL解包
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IByteStream Unwrap(IContext context, IByteStream input)
        {
            IByteStream output = PoolBufferAllocator.Default.AllocStream();
            _agent.ReadFromSslStream(output);
            return output;
        }
        /// <summary>
        /// 握手异步回调结果处理
        /// </summary>
        /// <param name="ar"></param>
        void AsyncCallback(IAsyncResult ar)
        {
            SslStream sslStream = ar.AsyncState as SslStream;
            try
            {
                sslStream.EndAuthenticateAsServer(ar);
            }
            catch (IOException)
            {
                _agent.State = SSLHandlerState.UnAuthentication;
                return;
            }
            catch (Exception)
            {
                _agent.State = SSLHandlerState.UnAuthentication;
                return;
            }
            _agent.State = SSLHandlerState.Authenticated;
        }
        /// <summary>
        /// 验证当前是否是已验证状态
        /// </summary>
        /// <returns></returns>
        bool EnsureAuthenticated()
        {
            SSLHandlerState oldState = _agent.State;
            if ((oldState & SSLHandlerState.AuthenticationStarted) == 0)
            {
                _agent.State = SSLHandlerState.Authenticating;
                _agent.BeginAuthenticateAsServer(AsyncCallback);
                return false;
            }
            return (oldState & SSLHandlerState.Authenticated) != 0;
        }



        public override void HandlerInstalled(IContext context) { }
        public override void HandlerUninstalled(IContext context) { }
    }

}
