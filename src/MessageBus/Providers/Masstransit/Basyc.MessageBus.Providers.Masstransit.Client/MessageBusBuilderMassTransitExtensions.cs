using Basyc.MessageBus.Client.Building;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Client.MasstTransit;

public static class MessageBusBuilderMassTransitExtensions
{
    /// <summary>
    /// Takes registered Basyc IRequestHandlers and wraps them with MassTransit IConsumers, Hosted by RabbitMQ.
    /// </summary>
    public static BusClientSetupProviderStage UseMassTransitProvider(this BusClientSetupProviderStage builder)
    {
        var services = builder.services;
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
