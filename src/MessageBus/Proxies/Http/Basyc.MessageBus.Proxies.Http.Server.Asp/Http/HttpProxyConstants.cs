using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basyc.MessageBus.HttpProxy.Server.Asp.Http;

public static class HttpProxyConstants
{
    public static readonly RequestDelegate ProxyHandler = async (context) =>
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
    };
}
