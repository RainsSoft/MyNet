using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Handlers;
using System;

namespace MyNet.Middleware.Http
{
    public interface IHttpHandlerState
    {
        void Read(IByteStream stream, IContext context, bool isssl);
        void Write(IContext context, object msg);
        void WriteFinish(IContext context, object msg, bool isssl);
        void Inactive(IContext context);
    }
}
