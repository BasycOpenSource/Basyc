using Basyc.MessageBus.Client.RequestResponse;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;

namespace Basyc.MessageBus.Client.MasstTransit;

public static class MassTransitConfiguratorExtensions
{
    public static void RegisterBasycHandlersAsMassTransitConsumers(this IServiceCollectionBusConfigurator busConfigurator)
    {
        var messageHandlerTypes = busConfigurator.Collection
         .Where(service => GenericsHelper.IsAssignableToGenericType(service.ServiceType, typeof(IMessageHandler<>)));

        foreach (var messageHandlerService in messageHandlerTypes)
        {
            Type handlerType = messageHandlerService.ImplementationType!;
            Type messageType = GenericsHelper.GetTypeArgumentsFromParent(handlerType, typeof(IMessageHandler<>))[0];
            Type proxyConsumerType = typeof(MassTransitBasycConsumerProxy<>).MakeGenericType(messageType);
            busConfigurator.AddConsumer(proxyConsumerType);
        }

        var messagesWithResponse = busConfigurator.Collection
            .Where(service => GenericsHelper.IsAssignableToGenericType(service.ServiceType, typeof(IMessageHandler<,>)));

        foreach (var messageHandlerServiceWithResponse in messagesWithResponse)
        {
            Type handlerType = messageHandlerServiceWithResponse.ImplementationType!;
            Type[] typeArguments = GenericsHelper.GetTypeArgumentsFromParent(handlerType, typeof(IMessageHandler<,>));
            Type messageType = typeArguments[0];
            Type responseType = typeArguments[1];
            Type proxyConsumerType = typeof(MassTransitBasycConsumerProxy<,>).MakeGenericType(messageType, responseType);
            busConfigurator.AddConsumer(proxyConsumerType);
        }
    }
}
