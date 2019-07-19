using MyNet.Loop.Scheduler;

namespace MyNet.Loop
{
    public abstract class AbstractRun : IRunnable
    {
        protected bool _disableRun;
        public bool DisableRun
        {
            get
            {
                return _disableRun;
            }

            set
            {
                _disableRun = value;
            }
        }
        public AbstractRun()
        {
            _disableRun = false;
        }

        public abstract void Run();
    }
}
