using MyNet.Channel;
using MyNet.Handlers;
using System;

namespace MyNet.Middleware.SSL
{
    public class SSLWritePacket : WritePacket
    {
        WritePacket _parent;
        public WritePacket Parent
        {
            get { return _parent; }
        }
        public SSLWritePacket(WritePacket parent) {
            _parent = parent;
        }
    }
}
