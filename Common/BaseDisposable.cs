using System;
namespace MyNet.Common
{
    /// <summary>
    /// 对象显示回收非托管资源时继承此类
    /// </summary>
    public abstract class BaseDisposable : IDisposable
    {
        ~BaseDisposable()
        {
            Dispose(false);
        }
        /// <summary>
        /// 显示释放对象资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            //此处开始释放非托管资源 
            OnUnManDisposed();
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        protected abstract void OnUnManDisposed();
    }
}
