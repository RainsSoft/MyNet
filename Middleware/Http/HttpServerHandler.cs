using MyNet.Buffer;
using MyNet.Channel;
using MyNet.Handlers;
using MyNet.Middleware.SSL;

namespace MyNet.Middleware.Http
{
    public class HttpServerHandler : AbstractChannelHandler, IStateSource
    {

        protected HttpConfig _config;
        protected IHttpHandlerState _currentState;
        public HttpServerHandler():this(new HttpConfig()) { }
        public HttpServerHandler(HttpConfig config)
        {
            _config = config;
            //初始为http
            _currentState = new HttpState(this, _config);
        }

        public override void ChannelRead(IContext context, object msg)
        {
            IByteStream stream = msg as IByteStream;
            if (stream != null)
            {
                bool isssl = false;
                //判断是否为SSL加密的请求
                if (msg is SSLUnwrapStream)
                {
                    isssl = true;
                }
                _currentState.Read(stream, context, isssl);
            }
        }
        public override void ChannelWrite(IContext context, object msg)
        {
            _currentState.Write(context, msg);
        }
        public override void ChannelWriteFinish(IContext context, object msg)
        {
            _currentState.WriteFinish(context, msg, false);
            context.FireNextWriteFinish(msg);
        }

        public override void ChannelInactive(IContext context)
        {
            if (!(context.Channel is ServerChannel))
            {
                _currentState.Inactive(context);
            }
            context.FireNextInactive();
        }

        public override void HandlerInstalled(IContext context)
        {
        }

        public override void HandlerUninstalled(IContext context)
        {
        }
        public void SetState(IHttpHandlerState state)
        {
            _currentState = state;
        }
    }
}
