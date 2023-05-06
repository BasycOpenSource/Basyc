using Basyc.MessageBus.Client.Building;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Client.MasstTransit;

public static class MessageBusBuilderMassTransitExtensions
{
    /// <summary>
    /// Takes registered Basyc IRequestHandlers and wraps them with MassTransit IConsumers, Hosted by RabbitMQ.
    /// </summary>
    public static BusClientSetupProviderStage UseMassTransitProvider(this BusClientSetupProviderStage builder)
    {
        var services = builder.Services;
        services.AddSingleton<ITypedMessageBusClient, MassTransitMessageBusClient>();
        services.AddHealthChecks();
        services.AddMassTransit(x =>
        {
            x.RegisterBasycHandlersAsMassTransitConsumers();
            x.UsingRabbitMq((transitContext, rabbitConfig) =>
            {
                rabbitConfig.ConfigureEndpoints(transitContext);
            });
        });
        return builder;
    }
}
