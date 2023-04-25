using Basyc.MessageBus.HttpProxy.Server.Asp.Http;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationHttpProxyExtensions
{
    public static WebApplication MapBasycHttpMessageBusProxy(this WebApplication app) => MapBasycHttpMessageBusProxy(app, string.Empty);

    public static WebApplication MapBasycHttpMessageBusProxy(this WebApplication app, string pattern)
    {
        app.MapPost(pattern, HttpProxyConstants.ProxyHandler);
        return app;
    }
}
