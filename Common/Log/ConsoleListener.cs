using System;
using System.Text;

namespace MyNet.Common.Log
{
    public class ConsoleListener : ILoggerListener
    {
        private string MakeNotify(string level, object msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("[{0}]", DateTime.Now.ToString("MM-dd HH:mm:ss")));
            sb.Append(string.Format("[{0}]", level));
            sb.Append(msg);
            return sb.ToString();
        }
        public void Error(object msg)
        {
            Console.WriteLine(MakeNotify("错误", msg));
        }
        public void Info(object msg)
        {
            Console.WriteLine(MakeNotify("信息", msg));
        }
        public void Debug(object msg)
        {
#if DEBUG
            Console.WriteLine(MakeNotify("调试", msg));
#endif
        }
    }
}
