
namespace MyNet.ClientConnection
{
    public interface IConnection
    {
        /// <summary>
        /// 是否连接
        /// </summary>
        bool Active { get; }
        /// <summary>
        /// 设为空闲
        /// </summary>
        void Close();
    }
}
