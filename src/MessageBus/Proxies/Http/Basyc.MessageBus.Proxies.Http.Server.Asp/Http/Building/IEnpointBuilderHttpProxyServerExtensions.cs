using Basyc.MessageBus.HttpProxy.Server.Asp.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IEnpointBuilderHttpProxyServerExtensions
    {

        public static void MapHttpMessageBusProxyServer(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("", async (HttpContext context) =>
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
    }
}