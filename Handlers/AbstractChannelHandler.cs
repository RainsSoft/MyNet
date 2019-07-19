using System;

namespace MyNet.Handlers
{
    public abstract class AbstractChannelHandler : IChannelHandler
    {
        public virtual void ChannelAccept(IContext context, object accepter)
        {
            context.FireNextAccept(accepter);
        }

        public virtual void ChannelActive(IContext context)
        {
            context.FireNextActive();
        }

        public virtual void ChannelInactive(IContext context)
        {
            context.FireNextInactive();
        }

        public virtual void ChannelRead(IContext context, object msg)
        {
            context.FireNextRead(msg);
        }

        public virtual void ChannelWrite(IContext context, object msg)
        {
            context.FirePreWrite(msg);
        }

        public virtual void ChannelWriteErr(IContext context, object msg)
        {
            context.FireNextWriteErr(msg);
        }

        public virtual void ChannelWriteFinish(IContext context, object msg)
        {
            context.FireNextWriteFinish(msg);
        }
   
        public abstract void HandlerInstalled(IContext context);

        public abstract void HandlerUninstalled(IContext context);
    }
}
