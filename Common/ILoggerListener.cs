using System;

namespace MyNet.Common
{
    public interface ILoggerListener
    {
        void Error(object msg);
        void Info(object msg);
        void Debug(object msg);
    }
}
