using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public class TwoRunnable<T1, T2> : AbstractRun
    {
        private Action<T1, T2> _ac;
        private T1 _state1;
        private T2 _state2;
        public TwoRunnable(Action<T1, T2> ac, T1 state1, T2 state2)
        {
            _ac = ac;
            _state1 = state1;
            _state2 = state2;
        }

        public override void Run()
        {
            _ac(_state1, _state2);
        }
    }
}
