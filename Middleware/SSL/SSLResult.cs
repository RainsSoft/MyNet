using System;
using System.Threading;

namespace MyNet.Middleware.SSL
{
    abstract class SSLResult<T>: IAsyncResult
    {
        public object AsyncState { get; set; }
        public abstract WaitHandle AsyncWaitHandle { get; }
        public abstract bool CompletedSynchronously { get; }
        public abstract bool IsCompleted { get; }
        public T Result { get; set; }
    }
}
