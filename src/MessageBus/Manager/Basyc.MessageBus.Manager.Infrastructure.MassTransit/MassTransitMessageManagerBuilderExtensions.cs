using Basyc.MessageBus.Manager.Application.Requesting;
using Basyc.MessageBus.Manager.Infrastructure;
using Basyc.MessageBus.Manager.Infrastructure.MassTransit;
using Microsoft.Extensions.DependencyInjection;

//using MassTransit;

namespace Basyc.MessageBus.Manager;

public static class MassTransitMessageManagerBuilderExtensions
{
	public static TypedProviderBuilder UseMasstransit(this BusManagerApplicationBuilder managerBuilder)
	{
		UseMasstransitReqeustClient(managerBuilder);
		var typedBuilder = managerBuilder.UseTypedProvider();
		return typedBuilder;
	}

	public static BusManagerApplicationBuilder UseMasstransitReqeustClient(this BusManagerApplicationBuilder managerBuilder)
	{
		managerBuilder.services.AddSingleton<IRequestHandler, MassTransitRequestHandler>();
		return managerBuilder;
	}
}
