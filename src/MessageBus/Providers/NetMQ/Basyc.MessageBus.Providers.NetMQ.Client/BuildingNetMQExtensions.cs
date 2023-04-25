using Basyc.MessageBus.Client;
using Basyc.MessageBus.Client.Building;
using Basyc.MessageBus.Client.NetMQ;
using Basyc.MessageBus.Client.NetMQ.Sessions;
using Basyc.MessageBus.Client.RequestResponse;
using Basyc.MessageBus.NetMQ.Shared;
using Basyc.MessageBus.Shared;
using Basyc.Serializaton.Abstraction;

namespace Microsoft.Extensions.DependencyInjection;

public static class BuildingNetMqExtensions
{
    private const int defaultBrokerServerPort = 5367;
    private const string defaultBrokerServerAddress = "localhost";

    public static BusClientUseDiagnosticsStage SelectNetMqProvider(this BusClientSetupProviderStage builder,
        int? brokerServerPort) => SelectNetMqProvider(builder);

    public static BusClientUseDiagnosticsStage SelectNetMqProvider(this BusClientSetupProviderStage builder,
        string? clientId = null,
        int brokerServerPort = defaultBrokerServerPort,
        string brokerServerAddress = defaultBrokerServerAddress)
    {
        var services = builder.Services;
        AddClients(services);

        services.AddSingleton<ISessionManager<NetMqSessionResult>, InMemorySessionManager<NetMqSessionResult>>();
        services.Configure<NetMqMessageBusClientOptions>(x =>
        {
            x.BrokerServerAddress = brokerServerAddress;
            x.BrokerServerPort = brokerServerPort;
            x.WorkerId = clientId;
        });

        services.AddBasycSerialization()
            .SelectProtobufNet();

        services.AddSingleton<INetMqMessageWrapper, NetMqMessageWrapper>();
        var areMessagesByte = services.FirstOrDefault(x => x.ServiceType == typeof(IByteMessageBusClient)) == null;

        AddMessageHandlerManager(services);
        return new BusClientUseDiagnosticsStage(services);
    }

    private static void AddMessageHandlerManager(IServiceCollection services)
    {
        services.AddSingleton<IMessageHandlerManager, MessageHandlerManager>();
        services.Configure<MessageHandlerManagerOptions>(handlerManagerOptions =>
        {
            handlerManagerOptions.IsDiagnosticLoggingEnabled = true;

            var messageHandlerTypes = services
                .Where(service => service.ServiceType.IsAssignableToGenericType(typeof(IMessageHandler<>)));

            foreach (var messageHandlerService in messageHandlerTypes)
            {
                //Type messageType = messageHandlerService.ServiceType!.GetTypeArgumentsFromParent(typeof(IMessageHandler<>))[0];
                var messageType = messageHandlerService.ServiceType.GetGenericArguments()[0];
                var handleMethodInfo = typeof(IMessageHandler<>).MakeGenericType(messageType).GetMethod(nameof(IMessageHandler<IMessage>.Handle))!;
                handlerManagerOptions.HandlerInfos.Add(new NetMqMessageHandlerInfo(TypedToSimpleConverter.ConvertTypeToSimple(messageType),
                    messageType,
                    handleMethodInfo));
            }

            var messagesWithResponse = services
                .Where(service => service.ServiceType.IsAssignableToGenericType(typeof(IMessageHandler<,>)));

            foreach (var messageHandlerServiceWithResponse in messagesWithResponse)
            {
                //Type[] typeArguments = messageHandlerServiceWithResponse.ServiceType!.GetTypeArgumentsFromParent(typeof(IMessageHandler<,>));
                var typeArguments = messageHandlerServiceWithResponse.ServiceType!.GetGenericArguments();
                var messageType = typeArguments[0];
                var responseType = typeArguments[1];
                var handleWithResponseMethodInfo = typeof(IMessageHandler<,>)
                    .MakeGenericType(messageType, responseType)
                    .GetMethod(nameof(IMessageHandler<IMessage<object>, object>.Handle))!;
                handlerManagerOptions.HandlerInfos.Add(new NetMqMessageHandlerInfo(TypedToSimpleConverter.ConvertTypeToSimple(messageType),
                    messageType,
                    responseType,
                    handleWithResponseMethodInfo));
            }
        });
    }

    private static void AddClients(IServiceCollection services)
    {
        services.AddSingleton<IByteMessageBusClient, NetMqByteMessageBusClient>();
        services.AddSingleton<ITypedMessageBusClient, TypedFromByteMessageBusClient>();
        services.AddSingleton<IObjectMessageBusClient, ObjectFromByteMessageBusClient>();
    }
}
