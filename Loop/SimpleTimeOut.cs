using System;

namespace MyNet.Loop
{
    public class SimpleTimeOut : TriggerRunnable
    {
        private Action _ac;
        public SimpleTimeOut(Action ac, int interval) : base(interval, 1)
        {
            _ac = ac;
        }

        public override void Run()
        {
            _ac();
        }
    }
}
