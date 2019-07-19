using MyNet.Channel;
using MyNet.Handlers;
using MyNet.Loop;
using MyNet.Loop.Scheduler;
using MyNet.Middleware.Http.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyNet.Middleware.Http
{
    public class DefaultWebSocketHandler : IdleStateHandler
    {
        private Queue<ITriggerRunnable> _timeout = new Queue<ITriggerRunnable>();
        public DefaultWebSocketHandler() : base(30, 0, 0) { }
        public DefaultWebSocketHandler(int pingpongtime) : base(pingpongtime, 0, 0)
        {
        }
        private void ClearTimeout(IContext ctx)
        {
            if (_timeout.Count > 0)
            {
                ctx.Loop.ClearRun(_timeout.Dequeue());
            }
        }
        public override void ChildRead(IContext context, object msg)
        {
            ChannelBase channel = context.Channel;
            FrameRequest request = msg as FrameRequest;
            if (request != null)
            {
                switch (request.Frame)
                {
                    case FrameCodes.Connected:
                        context.FireNextRead(msg);
                        channel.AddListener(CHANNEL_READ_IDLE, (EventArgs e) =>
        {
            string id = Guid.NewGuid().ToString();
            byte[] content = Encoding.UTF8.GetBytes(id);
            FrameResponse ping = new FrameResponse(FrameCodes.Ping, content);
            channel.SendAsync(ping);
            ITriggerRunnable run = new TimeOut<object>(it =>
            {
                context.Channel.Dispose();
            }, null, 2000);
            _timeout.Enqueue(run);
            context.Loop.Execute(run);
        });
                        break;
                    case FrameCodes.Close:
                        context.FireNextRead(msg);
                        channel.Dispose();
                        break;
                    case FrameCodes.Ping:
                        FrameResponse pong = new FrameResponse(FrameCodes.Pong, request.Content);
                        channel.SendAsync(pong);
                        break;
                    case FrameCodes.Pong:
                        ClearTimeout(context);
                        break;
                    default:
                        context.FireNextRead(msg);
                        break;
                }
            }
            else
            {
                context.FireNextRead(msg);
            }
        }


        public override void HandlerInstalled(IContext context)
        {
        }

        public override void HandlerUninstalled(IContext context)
        {
        }
    }
}
