using MyNet.Channel;
using System;
using MyNet.Loop;

namespace MyNet.Handlers
{
    public class DefaultChannelPipeline : IChannelPipeline
    {
        private ChannleHandlerContext _head;
        private ChannleHandlerContext _tail;
        public ChannleHandlerContext Head { get { return _head; } }
        public ChannleHandlerContext Tail { get { return _tail; } }
        private ChannelBase _channel;
        private bool _isReg;
        public ChannelBase Channel { get { return _channel; } }
        public DefaultChannelPipeline(ChannelBase channel)
        {
            _isReg = false;
            _channel = channel;
            _head = new HeadContext(_channel);
            _tail = new TailContext(_channel);
            _head.Next = _tail;
            _tail.Prev = _head;
        }

        public void FireChannelActive()
        {
            _isReg = true;
            _head.FireNextActive();
        }

        public void FireChannelInactive()
        {
            _head.FireNextInactive();
        }
        public void FireChannelRead(object msg)
        {
            _head.FireNextRead(msg);
        }
        public void FireChannelWrite(object msg)
        {
            _tail.FirePreWrite(msg);
        }
        public void FireChannelWriteFinish(object msg)
        {
            _head.FireNextWriteFinish(msg);
        }
        public void FireChannelWriteErr(object msg)
        {
            _head.FireNextWriteErr(msg);
        }
        public void FireChannelAccept(object accepter)
        {
            _head.FireNextAccept(accepter);
        }
        private void SyncAddFirst(IChannelHandler handler)
        {
            DefaultChannelHandlerContext newContext = new DefaultChannelHandlerContext(_channel, handler);
            ChannleHandlerContext nextContext = this._head.Next;
            newContext.Next = nextContext;
            newContext.Prev = this._head;
            this._head.Next = newContext;
            nextContext.Prev = newContext;

            newContext.Handler.HandlerInstalled(newContext);

        }
        public IChannelPipeline AddFirst(IChannelHandler handler)
        {
            if (!_isReg)
            {
                SyncAddFirst(handler);
                return this;
            }
            if (_channel.Loop.InCurrentThread())
            {
                SyncAddFirst(handler);
            }
            else
            {
                _channel.Loop.Execute(new DefaultRunnable<IChannelHandler>(SyncAddFirst, handler));
            }
            return this;
        }
        private void SyncAddFirst(params IChannelHandler[] handlers)
        {
            foreach (IChannelHandler ch in handlers)
            {
                SyncAddFirst(ch);
            }
        }
        public IChannelPipeline AddFirst(params IChannelHandler[] handlers)
        {
            if (!_isReg)
            {
                SyncAddFirst(handlers);
                return this;
            }
            if (_channel.Loop.InCurrentThread())
            {
                SyncAddFirst(handlers);
            }
            else
            {
                _channel.Loop.Execute(new SimpleRunnable(() =>
                {
                    SyncAddFirst(handlers);
                }));
            }
            return this;
        }
        private void SyncAddLast(IChannelHandler handler)
        {
            DefaultChannelHandlerContext newContext = new DefaultChannelHandlerContext(_channel, handler);
            ChannleHandlerContext prevContext = this._tail.Prev;
            newContext.Prev = prevContext;
            newContext.Next = this._tail;
            this._tail.Prev = newContext;
            prevContext.Next = newContext;

            newContext.Handler.HandlerInstalled(newContext);
        }
        public IChannelPipeline AddLast(IChannelHandler handler)
        {
            if (!_isReg)
            {
                SyncAddLast(handler);
                return this;
            }
            if (_channel.Loop.InCurrentThread())
            {
                SyncAddLast(handler);
            }
            else
            {
                _channel.Loop.Execute(new DefaultRunnable<IChannelHandler>(SyncAddLast, handler));
            }
            return this;
        }
        private void SyncAddLast(params IChannelHandler[] handlers)
        {
            foreach (IChannelHandler ch in handlers)
            {
                SyncAddLast(ch);
            }
        }
        public IChannelPipeline AddLast(params IChannelHandler[] handlers)
        {
            if (!_isReg)
            {
                SyncAddLast(handlers);
                return this;
            }
            if (_channel.Loop.InCurrentThread())
            {
                SyncAddLast(handlers);
            }
            else
            {
                _channel.Loop.Execute(new SimpleRunnable(() =>
                {
                    SyncAddLast(handlers);
                }));
            }
            return this;
        }
        private void SyncRemove(ChannleHandlerContext context)
        {
            ChannleHandlerContext prevContext = context.Prev;
            ChannleHandlerContext nextContext = context.Next;
            prevContext.Next = nextContext;
            nextContext.Prev = prevContext;

            context.Handler.HandlerUninstalled(context);
        }
        public IChannelPipeline Remove(ChannleHandlerContext context)
        {
            if (!_isReg)
            {
                SyncRemove(context);
                return this;
            }
            if (_channel.Loop.InCurrentThread())
            {
                SyncRemove(context);
            }
            else
            {
                _channel.Loop.Execute(new DefaultRunnable<ChannleHandlerContext>(SyncRemove, context));
            }
            return this;
        }
        private void SyncRemove(Type handlertype)
        {
            ChannleHandlerContext nextContext = _head.Next;
            while (nextContext != _tail)
            {
                if (handlertype.IsInstanceOfType(nextContext))
                {
                    SyncRemove(nextContext);
                }
                nextContext = nextContext.Next;
            }
        }
        public IChannelPipeline Remove(Type handlertype)
        {
            if (!_isReg)
            {
                SyncRemove(handlertype);
                return this;
            }
            if (_channel.Loop.InCurrentThread())
            {
                SyncRemove(handlertype);
            }
            else
            {
                _channel.Loop.Execute(new DefaultRunnable<Type>(SyncRemove, handlertype));
            }
            return this;
        }
        /// <summary>
        /// 从尾部开始删除
        /// </summary>
        /// <param name="ctx"></param>
        private void SyncClear(ChannleHandlerContext ctx)
        {
            for (;;)
            {
                if (ctx == this._head)
                {
                    break;
                }
                SyncRemove(ctx);
                ctx = ctx.Prev;
            }
        }
        public void Clear()
        {
            if (!_isReg)
            {
                SyncClear(this._tail.Prev);
            }
            if (_channel.Loop.InCurrentThread())
            {
                SyncClear(this._tail.Prev);
            }
            else
            {
                _channel.Loop.Execute(new SimpleRunnable(() =>
                {
                    SyncClear(this._tail.Prev);
                }));
            }
        }


    }
}
