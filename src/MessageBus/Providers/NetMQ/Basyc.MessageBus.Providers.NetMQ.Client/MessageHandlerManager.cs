using Basyc.Diagnostics.Shared;
using Basyc.MessageBus.Client.Diagnostics;
using Basyc.MessageBus.Client.RequestResponse;
using Basyc.MessageBus.NetMQ.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OneOf;

namespace Basyc.MessageBus.Client.NetMQ;

public class MessageHandlerManager : IMessageHandlerManager
{
    private readonly Dictionary<string, HandlerMetadata> handlerTypesCacheMap = new();
    private readonly IOptions<MessageHandlerManagerOptions> options;
    private readonly IServiceProvider serviceProvider;

    public MessageHandlerManager(IServiceProvider serviceProvider, IOptions<MessageHandlerManagerOptions> options)
    {
        this.serviceProvider = serviceProvider;
        this.options = options;

        foreach (var handlerInfo in options.Value.HandlerInfos)
        {
            Type handlerInterfaceType;
            Type handlerType;

            if (handlerInfo.HasResponse)
            {
                handlerInterfaceType = typeof(IMessageHandler<,>);
                handlerType = handlerInterfaceType.MakeGenericType(handlerInfo.MessageType, handlerInfo.ResponseType!);
            }
            else
            {
                handlerInterfaceType = typeof(IMessageHandler<>);
                handlerType = handlerInterfaceType.MakeGenericType(handlerInfo.MessageType);
            }

            HandlerMetadata newHandlerMetadata = new(handlerInfo, handlerType);
            handlerTypesCacheMap.Add(handlerInfo.MessageSimpleType, newHandlerMetadata);
        }
    }

    public async Task<OneOf<object, Exception>> ConsumeMessage(string messageType,
        object? messageData,
        string traceId,
        string parentId,
        CancellationToken cancellationToken)
    {
        if (handlerTypesCacheMap.TryGetValue(messageType, out var handlerMetadata) is false)
        {
            throw new InvalidOperationException("Handler for this message not found");
        }

        var handler = serviceProvider.GetRequiredService(handlerMetadata.HandlerRuntimeType)!;
        BusHandlerLoggerSessionManager.StartSession(new LoggingSession(traceId, handlerMetadata.HandlerInfo.HandleMethodInfo.Name));
        var invokeActivity = DiagnosticHelper.Start("Invoking method info");
        var handlerResultTask = (Task)handlerMetadata.HandlerInfo.HandleMethodInfo.Invoke(handler, new[] { messageData!, cancellationToken })!;
        object? handlerResult;
        try
        {
            if (handlerMetadata.HandlerInfo.HasResponse)
            {
                object taskResult = ((dynamic)handlerResultTask).Result!;
                invokeActivity.Stop();
                handlerResult = taskResult;
            }
            else
            {
                await handlerResultTask;
                invokeActivity.Stop();
                handlerResult = new VoidResult();
            }
        }
        catch (Exception ex)
        {
            var endSessionActivity2 = DiagnosticHelper.Start("Basyc.MessageBus.Client.NetMQ.MessageHandlerManager End BusHandlerLoggerSessionManager session");
            BusHandlerLoggerSessionManager.EndSession();
            endSessionActivity2.Stop();
            invokeActivity.Stop();
            return ex;
        }

        var endSessionActivity = DiagnosticHelper.Start("Basyc.MessageBus.Client.NetMQ.MessageHandlerManager End BusHandlerLoggerSessionManager session");
        BusHandlerLoggerSessionManager.EndSession();
        endSessionActivity.Stop();

        return handlerResult;
    }

    public string[] GetConsumableMessageTypes() => handlerTypesCacheMap.Values
            .Select(handlerMetadata => handlerMetadata.HandlerInfo.MessageSimpleType)
            .ToArray();

    private record HandlerMetadata(NetMqMessageHandlerInfo HandlerInfo, Type HandlerRuntimeType);
}
