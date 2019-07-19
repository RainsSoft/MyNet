using MyNet.Common;
using MyNet.Loop;
using System;
namespace MyNet.Handlers
{
    public class LoopTimeoutHandler : AbstractChannelHandler
    {
        private LoopTimeOut _scheduler;
        public class LoopTimeOut : TriggerRunnable
        {
            private string _evtname;
            private IContext _context;
            public IContext Context
            {
                get { return _context; }
                set { _context = value; }
            }
            public LoopTimeOut(int interval, string evtname) : base(interval, 1)
            {
                _evtname = evtname;
            }

            public override void Run()
            {
                if (!_context.Channel.Active)
                {
                    _context = null;
                    return;
                }
                _context.Channel.Emit(_evtname, EventArgs.Empty);
                _repeatCount = 1;
                _nextTime = Converter.Cast<long>(DateTime.Now) + _repeatInterval;
                _context.Loop.Execute(this);
            }
        }
        public LoopTimeoutHandler(int milliseconds, string evtname)
        {
            _scheduler = new LoopTimeOut(milliseconds, evtname);
        }
        public override void HandlerInstalled(IContext context)
        {
            _scheduler.Context = context;
            context.Loop.Execute(_scheduler);
        }

        public override void HandlerUninstalled(IContext context)
        {
            _scheduler.DisableRun = true;
        }
    }
}
