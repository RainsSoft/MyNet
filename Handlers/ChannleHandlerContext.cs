using System;
using MyNet.Channel;
using MyNet.Loop;

namespace MyNet.Handlers
{
    public abstract class ChannleHandlerContext : IContext
    {
        public ChannleHandlerContext Next { get; set; }
        public ChannleHandlerContext Prev { get; set; }
        protected ChannelBase _channel;
        public abstract IChannelHandler Handler { get; }

        public ChannleHandlerContext(ChannelBase channel)
        {
            _channel = channel;
        }

        public IEventLoop Loop
        {
            get { return _channel.Loop; }

        }
        public ChannelBase Channel
        {
            get { return _channel; }
        }
        public void FireNextActive()
        {
            IEventLoop loop = _channel.Loop;
            if (loop.InCurrentThread())
            {
                if (Next != null)
                {
                    Next.Handler.ChannelActive(Next);
                }
            }
            else
            {
                loop.Execute(new SimpleRunnable(() =>
                {
                    if (Next != null)
                    {
                        Next.Handler.ChannelActive(Next);
                    }
                }));
            }

        }

        public void FireNextInactive()
        {
            IEventLoop loop = _channel.Loop;
            if (loop.InCurrentThread())
            {
                if (Next != null)
                {
                    Next.Handler.ChannelInactive(Next);
                }
            }
            else
            {
                loop.Execute(new SimpleRunnable(() =>
                {
                    if (Next != null)
                    {
                        Next.Handler.ChannelInactive(Next);
                    }
                }));
            }
        }


        public void FireNextRead(object msg)
        {
            IEventLoop loop = _channel.Loop;
            if (loop.InCurrentThread())
            {
                if (Next != null)
                {
                    Next.Handler.ChannelRead(Next, msg);
                }
            }
            else
            {
                loop.Execute(new SimpleRunnable(() =>
                {
                    if (Next != null)
                    {
                        Next.Handler.ChannelRead(Next, msg);
                    }
                }));
            }
        }


        public void FirePreWrite(object msg)
        {
            IEventLoop loop = _channel.Loop;
            if (loop.InCurrentThread())
            {
                if (Prev != null)
                {
                    Prev.Handler.ChannelWrite(Prev, msg);
                }
            }
            else
            {
                loop.Execute(new SimpleRunnable(() =>
                {
                    if (Prev != null)
                    {
                        Prev.Handler.ChannelWrite(Prev, msg);
                    }
                }));
            }

        }
        public void FireNextWriteFinish(object msg)
        {
            IEventLoop loop = _channel.Loop;
            if (loop.InCurrentThread())
            {
                if (Next != null)
                {
                    Next.Handler.ChannelWriteFinish(Next, msg);
                }
            }
            else
            {
                loop.Execute(new SimpleRunnable(() =>
                {
                    if (Next != null)
                    {
                        Next.Handler.ChannelWriteFinish(Next, msg);
                    }
                }));
            }
        }
        public void FireNextWriteErr(object msg)
        {
            IEventLoop loop = _channel.Loop;
            if (loop.InCurrentThread())
            {
                if (Next != null)
                {
                    Next.Handler.ChannelWriteErr(Next, msg);
                }
            }
            else
            {
                loop.Execute(new SimpleRunnable(() =>
                {
                    if (Next != null)
                    {
                        Next.Handler.ChannelWriteErr(Next, msg);
                    }
                }));
            }

        }
        public void FireNextAccept(object accepter)
        {
            IEventLoop loop = _channel.Loop;
            if (loop.InCurrentThread())
            {
                if (Next != null)
                {
                    Next.Handler.ChannelAccept(Next, accepter);
                }
            }
            else
            {
                loop.Execute(new SimpleRunnable(() =>
                {
                    if (Next != null)
                    {
                        Next.Handler.ChannelAccept(Next, accepter);
                    }
                }));
            }

        }


    }
}
