using MyNet.Channel;
using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public delegate void EventHandler(EventArgs e);
    public interface IEventLoop : IExecutor
    {
        IEventLoopGroup Group { get; }
    }
}
