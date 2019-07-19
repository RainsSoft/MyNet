using MyNet.Handlers;
using MyNet.Middleware.Http.WebSocket;
using System;

namespace MyNet.Middleware.Http
{
    public interface IStateSource
    {
        void SetState(IHttpHandlerState state);
    }
}
