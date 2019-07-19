using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public class SimpleRunnable : AbstractRun
    {
        private Action _ac;
        public SimpleRunnable(Action ac)
        {
            _ac = ac;
        }

        public override void Run()
        {
            _ac();
        }
    }
}
