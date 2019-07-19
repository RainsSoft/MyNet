using System;

namespace MyNet.Handlers
{
    public interface IChannelHandler
    {
        /// <summary>
        /// 处理者的安装
        /// </summary>
        /// <param name="context"></param>
        void HandlerInstalled(IContext context);
        /// <summary>
        /// 处理者的卸载
        /// </summary>
        /// <param name="context"></param>
        void HandlerUninstalled(IContext context);
        void ChannelActive(IContext context);
        void ChannelInactive(IContext context);
        void ChannelRead(IContext context, object msg);
        void ChannelWrite(IContext context, object msg);
        void ChannelWriteFinish(IContext context, object msg);
        void ChannelWriteErr(IContext context, object msg);
        void ChannelAccept(IContext context, object accepter);
    }
}
