using System;

namespace MyNet.Loop.Scheduler
{
    public interface IRunnable
    {        
        /// <summary>
        /// 是否禁止执行
        /// </summary>
        bool DisableRun { get; set; }
        void Run();
    }
}
