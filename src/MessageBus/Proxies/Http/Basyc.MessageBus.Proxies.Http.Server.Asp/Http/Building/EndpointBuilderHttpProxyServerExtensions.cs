using Basyc.MessageBus.HttpProxy.Server.Asp.Http;

namespace Microsoft.Extensions.DependencyInjection;

public static class EndpointBuilderHttpProxyServerExtensions
{
    public static void MapHttpMessageBusProxyServer(this IEndpointRouteBuilder endpoints) => endpoints.MapPost(string.Empty, async context =>
        {
            var httpHandler = context.RequestServices.GetRequiredService<ProxyHttpRequestHandler>();
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                await httpHandler.Handle(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(ex.Message);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        });
}
