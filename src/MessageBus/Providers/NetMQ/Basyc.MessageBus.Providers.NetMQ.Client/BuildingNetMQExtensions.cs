using Basyc.MessageBus.Client;
using Basyc.MessageBus.Client.Building;
using Basyc.MessageBus.Client.NetMQ;
using Basyc.MessageBus.Client.NetMQ.Sessions;
using Basyc.MessageBus.Client.RequestResponse;
using Basyc.MessageBus.NetMQ.Shared;
using Basyc.MessageBus.Shared;
using Basyc.Serialization.Abstraction;
using Basyc.Serializaton.Abstraction;
using Basyc.Shared.Helpers;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class BuildingNetMQExtensions
{
	private const int defaultBrokerServerPort = 5367;
	private const string defaultBrokerServerAddress = "localhost";

	public static BusClientUseDiagnosticsStage SelectNetMQProvider(this BusClientSetupProviderStage builder,
	   int? brokerServerPort) =>
		SelectNetMQProvider(builder, null, defaultBrokerServerPort, defaultBrokerServerAddress);

	public static BusClientUseDiagnosticsStage SelectNetMQProvider(this BusClientSetupProviderStage builder,
		string? clientId = null, int brokerServerPort = defaultBrokerServerPort, string brokerServerAddress = defaultBrokerServerAddress)
	{
		var services = builder.services;
		AddClients(services);

		services.AddSingleton<ISessionManager<NetMQSessionResult>, InMemorySessionManager<NetMQSessionResult>>();
		services.Configure<NetMQMessageBusClientOptions>(x =>
		{
			x.BrokerServerAddress = brokerServerAddress;
			x.BrokerServerPort = brokerServerPort;
			x.WorkerId = clientId;
		});

		services.AddBasycSerialization()
			.SelectProtobufNet();

		services.AddSingleton<INetMQMessageWrapper, NetMQMessageWrapper>();
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
				.Where(service => GenericsHelper.IsAssignableToGenericType(service.ServiceType, typeof(IMessageHandler<>)));

			foreach (var messageHandlerService in messageHandlerTypes)
			{
				//Type messageType = messageHandlerService.ServiceType!.GetTypeArgumentsFromParent(typeof(IMessageHandler<>))[0];
				Type messageType = messageHandlerService.ServiceType.GetGenericArguments()[0];
				MethodInfo handleMethodInfo = typeof(IMessageHandler<>).MakeGenericType(messageType).GetMethod(nameof(IMessageHandler<IMessage>.Handle))!;
				handlerManagerOptions.HandlerInfos.Add(new NetMQMessageHandlerInfo(TypedToSimpleConverter.ConvertTypeToSimple(messageType), messageType, handleMethodInfo));
			}

			var messagesWithResponse = services
				.Where(service => GenericsHelper.IsAssignableToGenericType(service.ServiceType, typeof(IMessageHandler<,>)));

			foreach (var messageHandlerServiceWithResponse in messagesWithResponse)
			{
				//Type[] typeArguments = messageHandlerServiceWithResponse.ServiceType!.GetTypeArgumentsFromParent(typeof(IMessageHandler<,>));
				Type[] typeArguments = messageHandlerServiceWithResponse.ServiceType!.GetGenericArguments();
				Type messageType = typeArguments[0];
				Type responseType = typeArguments[1];
				MethodInfo handleWithResponseMethodInfo = typeof(IMessageHandler<,>)
				.MakeGenericType(messageType, responseType)
				.GetMethod(nameof(IMessageHandler<IMessage<object>, object>.Handle))!;
				handlerManagerOptions.HandlerInfos.Add(new NetMQMessageHandlerInfo(TypedToSimpleConverter.ConvertTypeToSimple(messageType), messageType, responseType, handleWithResponseMethodInfo));
			}
		});
	}

	private static void AddClients(IServiceCollection services)
	{
		services.AddSingleton<IByteMessageBusClient, NetMQByteMessageBusClient>();
		services.AddSingleton<ITypedMessageBusClient, TypedFromByteMessageBusClient>();
		services.AddSingleton<IObjectMessageBusClient, ObjectFromByteMessageBusClient>();
	}
}
