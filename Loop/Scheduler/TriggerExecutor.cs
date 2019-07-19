using MyNet.Common;
using System;
namespace MyNet.Loop.Scheduler
{
    /// <summary>
    /// 执行触发任务
    /// </summary>
    public abstract class TriggerExecutor : IExecutor
    {
        private BinaryHeap<ITriggerRunnable> mTriggers = new BinaryHeap<ITriggerRunnable>();
       
        /// <summary>
        /// 弹出下一个到期触发器
        /// </summary>
        /// <returns></returns>
        protected ITriggerRunnable PopNextTrigger()
        {
            lock (mTriggers)
            {
                ITriggerRunnable tr = mTriggers.Peek();
                if (tr == null) return null;
                long nowticks = Converter.Cast<long>(DateTime.Now);
                if (tr.DeadTime <= nowticks)
                {
                    tr = mTriggers.Dequeue();
                    if (tr.RepeatCount > 0) tr.RepeatCount--;
                    if (tr.RepeatCount != 0)
                    {
                        tr.DeadTime = nowticks + tr.RepeatInterval;
                        mTriggers.Enqueue(tr);
                    }
                    return tr;
                }
                else
                {
                    return null;
                }
            }
        }
        protected void EnqueueTrigger(ITriggerRunnable trigger)
        {
            lock (mTriggers)
            {
                mTriggers.Enqueue(trigger);
            }
        }
        protected ITriggerRunnable PeekTrigger()
        {
            lock (mTriggers)
            {
                return mTriggers.Peek();
            }
        }
        public abstract void ClearRun(IRunnable run);
        public abstract bool InCurrentThread();
        public abstract void Execute(IRunnable task);
        public abstract void Close();

    }
}
