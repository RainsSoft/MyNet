using System;

namespace MyNet.Loop.Scheduler
{
    public interface IExecutor
    {
        void Close();
        bool InCurrentThread();
        void ClearRun(IRunnable run);
        /// <summary>
        /// 压入队列执行
        /// </summary>
        /// <param name="task"></param>
        void Execute(IRunnable task);
    }
}
