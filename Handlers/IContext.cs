using MyNet.Channel;
using MyNet.Loop;
using System;

namespace MyNet.Handlers
{
    public interface IContext
    {

        ChannelBase Channel { get; }
        IEventLoop Loop { get; }

        void FireNextActive();
        void FireNextInactive();
        void FireNextRead(object msg);
        void FirePreWrite(object msg);
        void FireNextWriteFinish(object msg);
        void FireNextWriteErr(object msg);
        void FireNextAccept(object accepter);
    }
}
