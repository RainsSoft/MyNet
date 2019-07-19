using System.Net.Sockets;
using MyNet.Loop;
using MyNet.Handlers;
using System.Threading;

namespace MyNet.Channel
{
    public abstract class AbstractChannel : ChannelBase
    {
        internal AutoResetEvent _waitResetEvent;
        protected int _active;
        protected IChannelPipeline _pipe;
        public override bool Active
        {
            get { return _active == 1; }
        }
        public override IChannelPipeline Pipeline
        {
            get { return _pipe; }
        }

        protected IEventLoop _loop;
        protected IChannelConfig _config;

        public AbstractChannel(Socket socket)
        {
            _pipe = new DefaultChannelPipeline(this);
            _config = new DefaultChannelConfig(socket);
            _active = 0;
        }

        public override IChannelConfig Config
        {
            get
            {
                return _config;
            }
        }
        public override IEventLoop Loop
        {
            get
            {
                return _loop;
            }
        }
        public override void RegisterLoop(IEventLoop loop)
        {
            _loop = loop;
            if (Interlocked.CompareExchange(ref _active, 1, 0) == 0)
            {
                OnBeforeActive();
                _pipe.FireChannelActive();
                OnAfterActive();
            }
        }

        protected override void OnUnManDisposed()
        {
            if (Interlocked.CompareExchange(ref _active, 0, 1) == 1)
            {
                if (_loop.InCurrentThread())
                {
                    _syncexe.Release();
                }
                else
                {
                    _loop.Execute(new SimpleRunnable(_syncexe.Release));
                }
            }
        }

        /// <summary>
        /// 阻止当前进程，Channel关闭才继续
        /// </summary>
        public override void Wait()
        {
            if (_waitResetEvent == null)
            {
                _waitResetEvent = new AutoResetEvent(false);
            }
            _waitResetEvent.WaitOne();
        }
        /// <summary>
        /// 触发激活前
        /// </summary>
        protected abstract void OnBeforeActive();
        /// <summary>
        /// 触发激活后
        /// </summary>
        protected abstract void OnAfterActive();

        public abstract class AbstractSyncChannel : SyncChannel
        {
            public AbstractSyncChannel()
            {
            }
            public override void Release()
            {
                Channel.Pipeline.FireChannelInactive();
                RemoveAllListeners();
                AbstractChannel c = (AbstractChannel)Channel;
                if (c._waitResetEvent != null)
                {
                    c._waitResetEvent.Set();
                }
            }
        }
    }
}
