using System;
using MyNet.Buffer;
using MyNet.Handlers;
using MyNet.Channel;
using MyNet.Loop.Scheduler;
using MyNet.Middleware.Http.WebSocket;
using MyNet.Loop;

namespace MyNet.Middleware.Http
{
    public class HttpState : IHttpHandlerState
    {
        protected const string WEB_SECKEY = "Sec-WebSocket-Key";
        protected ITriggerRunnable _timeout;
        protected IStateSource _source;
        protected HttpConfig _config;
        protected bool _keepalive;
        public HttpState(IStateSource source, HttpConfig config)
        {
            _source = source;
            _config = config;
            _keepalive = false;
        }

        protected void ClearTimeOut(ChannelBase channel)
        {
            if (_timeout != null)
            {
                channel.Loop.ClearRun(_timeout);
                _timeout = null;
            }
        }
        public void Read(IByteStream stream, IContext context, bool isssl)
        {
            ChannelBase channel = context.Channel;
            HttpRequestParseResult result = HttpRequestParser.Parse(context, stream, isssl);
            if (!result.IsHttp)
            {
                context.FireNextRead(stream);
                return;
            }

            //清除超时定时器
            ClearTimeOut(channel);
            //Post请求太大
            if (_config.MaxPostLen > 0 && result.ContentLength > _config.MaxPostLen)
            {
                new HttpResponse().End413(context);
                return;
            }

            // 数据未完整
            if (result.Request == null)
            {
                channel.MergeRead();
                return;
            }
            //Url太长
            if (result.Request.Path.Length > _config.MaxUrlLen)
            {
                new HttpResponse().End414(context);
                return;
            }

            if (result.Request.HttpMethod == HttpMethod.OPTIONS)
            {
                HttpResponse errreturn = new HttpResponse();
                errreturn.Headers.Add("Access-Control-Allow-Origin", "*"); //支持的域名
                errreturn.Headers.Add("Access-Control-Allow-Methods", "POST,GET,OPTIONS");//支持的http动作
                errreturn.Headers.Add("Access-Control-Allow-Headers", "Content-Type,Accept,Origin,User-Agent,DNT,Cache-Control,X-Mx-ReqToken,X-Data-Type,X-Requested-With");
                errreturn.End204(context);
                return;
            }

            // 协议升级成websocket
            if (result.Request.IsWebsocketRequest())
            {
                string secValue = result.Request.Headers[WEB_SECKEY];
                HandshakeResponse handshakeResponse = new HandshakeResponse(result.Request, secValue);
                channel.SendAsync(handshakeResponse);
                return;
            }
            _keepalive = result.Request.IsKeepAlive();
            //开始action处理
            context.FireNextRead(result.Request);
        }

        public void Write(IContext context, object msg)
        {
            HttpResponse response = msg as HttpResponse;
            if (response != null)
            {
                bool isContinue = true;
                if (_config.StatusListener != null && response.Status != 200)
                {
                    isContinue = _config.StatusListener(response);
                }
                if (isContinue)
                {
                    response.KeepAlive = _keepalive;
                    if (_config.DisableGzip)
                    {
                        response.Gzip = false;
                    }
                    HttpResponseEncoder.WriteResponse(response);
                    context.FirePreWrite(msg);
                }
            }
            else
            {
                HandshakeResponse websockethandshake = msg as HandshakeResponse;
                if (websockethandshake != null)
                {
                    HttpResponseEncoder.WriteWebSocketHandshakeResponse(websockethandshake);
                    //升级协议为websocket
                    _source.SetState(new WebSocketState(context, _source, _config));
                }
                context.FirePreWrite(msg);
            }

        }

        public void WriteFinish(IContext context, object msg, bool isssl)
        {
            ChannelBase channel = context.Channel;
            HttpResponse response = msg as HttpResponse;
            if (response != null)
            {
                if (_keepalive)
                {
                    ClearTimeOut(channel);
                    ///指定时间后关闭连接
                    _timeout = new TimeOut<IContext>(c =>
                    {
                        c.Channel.Dispose();
                    }, context, _config.KeepTime * 1000);
                    channel.Loop.Execute(_timeout);
                }
                else
                {
                    context.Channel.Dispose();
                }
                if (isssl)
                {
                    response.Dispose();
                }
            }
        }
        public void Inactive(IContext context)
        {
            ClearTimeOut(context.Channel);
        }

    }
}
