using System;
using MyNet.Loop;
using MyNet.Handlers;
using MyNet.Common;
using System.Net;
using System.Collections.Generic;

namespace MyNet.Channel
{
    public delegate void ChannelHandler(EventArgs e);
    public abstract class ChannelBase : BaseDisposable
    {
        protected abstract SyncChannel _syncexe { get; }
        protected ChannelID _id;
        public ChannelID Id
        {
            get
            {
                return _id;
            }
        }
        /// <summary>
        /// 属性值
        /// 暂存数据用
        /// </summary>
        public PropertyCollection Propertys { get; set; }
        public abstract EndPoint RemoteEndPoint { get; }
        public abstract EndPoint LocalEndPoint { get; }
        /// <summary>
        /// 激活状态，表示是否活动状态
        /// </summary>
        public abstract bool Active { get; }
        public abstract IChannelConfig Config { get; }
        public abstract IChannelPipeline Pipeline { get; }
        public abstract IEventLoop Loop { get; }
        public abstract void RegisterLoop(IEventLoop loop);
        public abstract void Wait();
    

        public ChannelBase()
        {
            _id = new ChannelID();
            Propertys = new PropertyCollection();
        }
   
        /// <summary>
        /// 清除所有绑定事件
        /// </summary>
        /// <param name="name"></param>
        public void RemoveAllListeners()
        {
            if (Loop.InCurrentThread())
            {
                _syncexe.RemoveAllListeners();
            }
            else
            {
                Loop.Execute(new SimpleRunnable(_syncexe.RemoveAllListeners));
            }
        }
        public void RemoveListener(string name)
        {
            if (Loop.InCurrentThread())
            {
                _syncexe.RemoveListener(name);
            }
            else
            {
                Loop.Execute(new DefaultRunnable<string>(_syncexe.RemoveListener, name));
            }
        }
        /// <summary>
        /// 绑定事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void AddListener(string name, ChannelHandler handler)
        {
            if (Loop.InCurrentThread())
            {
                _syncexe.AddListener(name, handler);
            }
            else
            {
                Loop.Execute(new TwoRunnable<string, ChannelHandler>(_syncexe.AddListener, name, handler));
            }
        }

   
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="e"></param>
        public void Emit(string name, EventArgs e)
        {
            if (Loop.InCurrentThread())
            {
                _syncexe.Emit(name, e);
            }
            else
            {
                Loop.Execute(new TwoRunnable<string, EventArgs>(_syncexe.Emit, name, e));
            }

        }
        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendAsync(object msg)
        {
            Pipeline.FireChannelWrite(msg);
        }
        /// <summary>
        /// 合并读取的数据包
        /// </summary>
        public void MergeRead()
        {
            _syncexe.MergeRead();
        }
        internal void WriteAsync(object packet)
        {
            _syncexe.WriteAsync(packet);
        }
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
        public bool Equals(ChannelBase channel)
        {
            if (channel == null) return false;
            return _id.Equals(channel.Id);
        }
        public override bool Equals(object obj)
        {
            ChannelBase target = obj as ChannelBase;
            return this.Equals(target);
        }
        public static bool operator ==(ChannelBase c1, ChannelBase c2)
        {
            if (Equals(c1, null) || Equals(c2, null))
            {
                return Equals(c1, c2);
            }
            return c1.Id.Equals(c2.Id);
        }
        public static bool operator !=(ChannelBase c1, ChannelBase c2)
        {
            if (Equals(c1, null) || Equals(c2, null))
            {
                return !Equals(c1, c2);
            }
            return !c1.Id.Equals(c2.Id);
        }

        public abstract class SyncChannel
        {
            protected Dictionary<string, ChannelHandler> _handlers;
            public abstract ChannelBase Channel { get; }
            public SyncChannel()
            {
                _handlers = new Dictionary<string, ChannelHandler>();
            }
           
            public void RemoveAllListeners()
            {
                _handlers.Clear();
            }
            public void RemoveListener(string name)
            {
                ChannelHandler h;
                if (_handlers.TryGetValue(name, out h))
                {
                    h = null;
                    _handlers.Remove(name);
                }
            }
            public void AddListener(string name, ChannelHandler handler)
            {
                ChannelHandler h;
                if (_handlers.TryGetValue(name, out h))
                {
                    h += handler;
                }
                else
                {
                    _handlers.Add(name, handler);
                }
            }
            public void Emit(string name, EventArgs e)
            {
                ChannelHandler h;
                if (_handlers.TryGetValue(name, out h))
                {
                    h(e);
                }
            }
            public abstract void MergeRead();
            public abstract void WriteAsync(object packet);
            public abstract void Release();
        }
    }
}
