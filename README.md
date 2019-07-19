# MyNet
类似Netty的事件驱动的网络应用程序框架。你可以当成是Netty的.net版本，但有些差别。

# 使用例子：

``` javascript
            EventLoopGroup acceptGroup = new EventLoopGroup();
            EventLoopGroup workGroup = new EventLoopGroup(10);
            ServerBootstrap boot = new ServerBootstrap();
            boot.Group(acceptGroup, workGroup);
            boot.Handler(new InitializerHandler(c =>
            {
                c.Pipeline.AddLast(new HttpServerHandler())
            }));
            boot.LaunchChannel("127.0.0.1", 5555).Wait();
```
