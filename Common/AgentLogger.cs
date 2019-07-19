using MyNet.Common.Log;
namespace MyNet.Common
{
    public class AgentLogger
    {

        private static string VersionInfo = string.Format(
   @"///////////////////////////////////////////////////////////////////////////
　◆◆◆　◆◆◆　　　　　　◆◆　◆◆◆　版本 {0}　　　　　　　　　　　   
　　◆◆　◆◆　　　　　　　　◆　　◆ 　 {1}　　　　　　　
　　◆◆　◆◆　　　　　　　　◆◆　◆　　　　　　　　　◆　　   
　　◆◆　◆◆　◆◆　　◆◆　◆◆　◆　　　◆◆　　　◆◆◆◆　
　　◆　◆　◆　　◆　　◆　　◆　◆◆　　◆　　◆　　　◆ 　　
　　◆　◆　◆　　◆　　◆　　◆　◆◆　　◆◆◆◆　　　◆ 　　
　　◆　◆　◆　　　◆◆　　　◆　　◆　　◆　　　　　　◆　　　
　◆◆　◆　◆◆　　　◆　　◆◆◆　◆　　　◆◆◆　　　◆◆◆　
　　　　　　　　　　◆　　　　　　　　　　　　　　　　　　　　　
　　　　　　　　◆◆　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　        
///////////////////////////////////////////////////////////////////////////", Utility.VERSION, Utility.COPYRIGHT);
        private volatile static AgentLogger _instance = null;
        private static object _lockobj = new object();
        private ILoggerListener _loggerlistener;
        private AgentLogger()
        {
            _loggerlistener = new ConsoleListener();
        }
        public static AgentLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockobj)
                    {
                        if (_instance == null)
                            _instance = new AgentLogger();
                    }
                }
                return _instance;
            }
        }


        public void Err(string msg)
        {
            _loggerlistener.Error(msg);
        }
        public void Info(string msg)
        {
            _loggerlistener.Info(msg);
        }
        public void Debug(string msg)
        {
            _loggerlistener.Debug(msg);
        }
        public void Logo()
        {
            _loggerlistener.Info(VersionInfo);
        }
        public void Change(ILoggerListener listener)
        {
            _loggerlistener = listener;
        }
    }
}
