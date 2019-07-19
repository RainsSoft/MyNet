using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public class ThreeRunnable<T1, T2, T3> : AbstractRun
    {
        private Action<T1, T2, T3> _ac;
        private T1 _state1;
        private T2 _state2;
        private T3 _state3;
        public ThreeRunnable(Action<T1, T2, T3> ac, T1 state1, T2 state2, T3 state3)
        {
            _ac = ac;
            _state1 = state1;
            _state2 = state2;
            _state3 = state3;
        }

        public override void Run()
        {
            _ac(_state1, _state2, _state3);
        }
    }
}
