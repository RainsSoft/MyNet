using System;
using System.Threading;

namespace MyNet.Middleware.SSL
{
    sealed class SSLAsyncResult<T> : SSLResult<T>
    {
        bool _isCompleted = false;
        AutoResetEvent _autoEvent = new AutoResetEvent(false);
        public void Finish()
        {
            if (Callback != null)
            {
                Callback.Invoke(this);
                _isCompleted = true;
            }
            else
            {
                throw new NotImplementedException("ReadCallback不能为空");
            }
        }
        public ArraySegment<byte> SSLAsyncBuffer { get; set; }
        public AsyncCallback Callback { get; set; }
        public override WaitHandle AsyncWaitHandle
        {
            get
            {
                return _autoEvent;
            }
        }

        public override bool CompletedSynchronously
        {
            get
            {
                return false;
            }
        }

        public override bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
        }
    }
}
