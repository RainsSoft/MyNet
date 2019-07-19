using MyNet.Loop.Scheduler;
namespace MyNet.Loop
{
    public class EventLoop : SingleThreadExecutor, IEventLoop
    {
        private IEventLoopGroup mGroup;

        public IEventLoopGroup Group
        {
            get
            {
                return mGroup;
            }
        }

        public EventLoop(IEventLoopGroup group)
        {
            mGroup = group;
        }
    }
}
