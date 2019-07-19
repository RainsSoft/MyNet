using MyNet.Common;
using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Loop
{
    public abstract class TriggerRunnable : ITriggerRunnable
    {
        protected bool _disableRun;
        protected long _nextTime;
        protected int _repeatInterval;
        protected int _repeatCount;
        /// <summary>
        /// 默认值设置
        /// </summary>
        /// <param name="interval">单位毫秒</param>
        /// <param name="repeat"></param>
        public TriggerRunnable(int interval, int repeat)
        {
            _nextTime = Converter.Cast<long>(DateTime.Now) + interval;
            _repeatInterval = interval;
            _repeatCount = repeat;
            _disableRun = false;
        }
        public long DeadTime
        {
            get
            {
                return _nextTime;
            }

            set
            {
                _nextTime = value;
            }
        }

        public int RepeatCount
        {
            get
            {
                return _repeatCount;
            }
            set
            {
                _repeatCount = value;
            }
        }

        public int RepeatInterval
        {
            get
            {
                return _repeatInterval;
            }

        }

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

        public abstract void Run();

        public int CompareTo(object obj)
        {
            ITriggerRunnable cmp = obj as ITriggerRunnable;
            if (cmp == null)
            {
                return -1;
            }
            return this._nextTime.CompareTo(cmp.DeadTime);
        }


   
    }
}
