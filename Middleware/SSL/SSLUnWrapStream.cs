using MyNet.Buffer;
using System;

namespace MyNet.Middleware.SSL
{
    public class SSLUnwrapStream : AgentByteStream
    {
        public SSLUnwrapStream(IByteStream stream) : base(stream)
        {
        }
    }
}
