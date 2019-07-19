using System;
using System.Collections.Concurrent;
using System.Threading;
using MyNet.Common;


namespace MyNet.Loop.Scheduler
{
    public class SingleThreadExecutor : TriggerExecutor
    {
        protected ConcurrentQueue<IRunnable> mTaskQueue;
        protected Thread mThread;
        protected ManualResetEvent mEmptySemaphore;
        protected bool mIsShuttingDown;


        public SingleThreadExecutor()
        {
            mIsShuttingDown = false;
            mEmptySemaphore = new ManualResetEvent(false);
            mTaskQueue = new ConcurrentQueue<IRunnable>();
            StartLoop();
        }
        protected void StartLoop()
        {
            if (mThread == null)
            {
                mThread = new Thread(this.Loop);
                mThread.Start();
            }
        }
        protected void Loop(object state)
        {
            while (!mIsShuttingDown)
            {
                FetchFromTriggerTask();
                IRunnable rn = WaitTask();
                while (rn != null)
                {
                    try
                    {
                        if (!rn.DisableRun)
                        {
                            rn.Run();
                        }
                    }
                    catch (Exception ex)
                    {
                        AgentLogger.Instance.Err(string.Format("任务{0}出错：{1},堆栈{2}", rn, ex.Message, ex.StackTrace));
                    }

                    rn = WaitTask();
                }
            }
            mThread = null;
        }
        public override void ClearRun(IRunnable run)
        {
            if (mThread == Thread.CurrentThread)
            {
                run.DisableRun = true;
            }
            else
            {
                this.Execute(new SimpleRunnable(() =>
                {
                    run.DisableRun = true;
                }));
            }
        }
        /// <summary>
        /// 获取当前到期的定时任务并放入任务队列
        /// </summary>
        protected void FetchFromTriggerTask()
        {
            ITriggerRunnable triggerTask = this.PopNextTrigger();
            while (triggerTask != null)
            {
                mTaskQueue.Enqueue(triggerTask);
                triggerTask = this.PopNextTrigger();
            }
        }

        protected IRunnable WaitTask()
        {
            IRunnable task;
            if (!mTaskQueue.TryDequeue(out task))
            {
                mEmptySemaphore.Reset();
                if (!mTaskQueue.TryDequeue(out task) && !mIsShuttingDown)
                {
                    ITriggerRunnable nextTriggerTask = this.PeekTrigger();
                    if (nextTriggerTask != null)
                    {
                        long delayticks = nextTriggerTask.DeadTime - Converter.Cast<long>(DateTime.Now);
                        if (delayticks > 0)
                        {
                            mEmptySemaphore.WaitOne((int)Math.Min(delayticks, int.MaxValue - 1));
                        }
                    }
                    else
                    {
                        mEmptySemaphore.WaitOne();
                    }

                }

            }
            return task;
        }
        /// <summary>
        /// 将要执行的任务压入任务队列
        /// </summary>
        /// <param name="task"></param>
        public override void Execute(IRunnable task)
        {
            ITriggerRunnable trigger = task as ITriggerRunnable;
            if (trigger != null)
            {
                EnqueueTrigger(trigger);
            }
            else
            {
                mTaskQueue.Enqueue(task);
            }
            if (mThread != Thread.CurrentThread)
            {
                mEmptySemaphore.Set();
            }
        }
        public override bool InCurrentThread()
        {
            return mThread == Thread.CurrentThread;
        }
        public override void Close()
        {
            mIsShuttingDown = true;
            mEmptySemaphore.Set();
        }


    }
}
