using Basyc.MessageBus.HttpProxy.Server.Asp.Http;

namespace Microsoft.Extensions.DependencyInjection;

public static class EndpointBuilderHttpProxyServerExtensions
{
    public static void MapHttpMessageBusProxyServer(this IEndpointRouteBuilder endpoints) => endpoints.MapPost(string.Empty, async context =>
        {
            var httpHandler = context.RequestServices.GetRequiredService<ProxyHttpRequestHandler>();
            try
            {
                await httpHandler.Handle(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(ex.Message);
            }
        });
}
