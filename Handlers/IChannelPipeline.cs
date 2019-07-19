using MyNet.Channel;
using System;

namespace MyNet.Handlers
{
    public interface IChannelPipeline
    {
       ChannelBase Channel { get; }
        ChannleHandlerContext Head { get; }
        ChannleHandlerContext Tail { get; }
        IChannelPipeline AddFirst(IChannelHandler handler);
        IChannelPipeline AddFirst(params IChannelHandler[] handlers);
        IChannelPipeline AddLast(IChannelHandler handler);
        IChannelPipeline AddLast(params IChannelHandler[] handlers);
        IChannelPipeline Remove(Type handlertype);
        void FireChannelRead(object msg);
        void FireChannelWrite(object msg);
        void FireChannelWriteFinish(object msg);
        void FireChannelWriteErr(object msg);
        void FireChannelAccept(object accepter);
        void FireChannelActive();
        void FireChannelInactive();
        void Clear();
    }
}
