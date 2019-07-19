using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public class DefaultRunnable<T> : AbstractRun
    {
        private Action<T> _ac;
        private T _state;
        public DefaultRunnable(Action<T> ac, T state)
        {
            _ac = ac;
            _state = state;
        }

        public override void Run()
        {
            _ac(_state);
        }
    }
}
