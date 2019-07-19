using System;
using System.Threading;

namespace MyNet.Middleware.SSL
{
    sealed class SSLSyncResult<T> : SSLResult<T>
    {
        public override bool IsCompleted { get { return true; } }

        public override WaitHandle AsyncWaitHandle
        {
            get { throw new InvalidOperationException("Cannot wait on a synchronous result."); }
        }


        public override bool CompletedSynchronously { get { return true; } }
    }
}
