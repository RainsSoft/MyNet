using System;

namespace MyNet.Loop
{
    public class TimeOut<T> : TriggerRunnable
    {
        private Action<T> _ac;
        private T _state;
        /// <summary>
        /// 超时
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="state"></param>
        /// <param name="interval">单位毫秒</param>
        public TimeOut(Action<T> ac, T state, int interval) : base(interval, 1)
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
