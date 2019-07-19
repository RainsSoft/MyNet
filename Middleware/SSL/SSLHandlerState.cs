using System;

namespace MyNet.Middleware.SSL
{
    public enum SSLHandlerState
    {
        Authenticating = 1,
        Authenticated = 1 << 1,
        UnAuthentication = 1 << 2,
        AuthenticationStarted = Authenticating | Authenticated,
    }
}
