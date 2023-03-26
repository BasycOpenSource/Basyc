using Basyc.DependencyInjection;
using Basyc.MessageBus.Client.RequestResponse;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Basyc.MessageBus.Client.Building;

public class BusClientSetupHandlersStage : BuilderStageBase
{
	public BusClientSetupHandlersStage(IServiceCollection services) : base(services)
	{
	}

	public BusClientSetupProviderStage NoHandlers()
	{
		return new BusClientSetupProviderStage(services);
	}

	public BusClientSetupProviderStage RegisterHandlersFromAssembly<THandlerAssemblyMarker>()
	{
		return RegisterHandlersFromAssembly(typeof(THandlerAssemblyMarker).Assembly);
	}

	public BusClientSetupProviderStage RegisterHandlersFromAssembly(params Assembly[] assembliesToScan)
	{
		foreach (var assembly in assembliesToScan)
		{
			var typesInAssembly = assembly.GetTypes();
			var handlerTypesInAssembly = typesInAssembly.Where(x => x.IsAssignableToGenericType(typeof(IMessageHandler<>)));
			foreach (var handlerType in handlerTypesInAssembly)
			{

				//var serviceType = typeof(IMessageHandler<>).MakeGenericType(handlerType.GetTypeArgumentsFromParent(typeof(IMessageHandler<>)));
				//services.AddScoped(serviceType, serviceProvider => RegisterHandlerHelper.CreateHandlerWithDecoratedLogger(handlerType, serviceProvider));
				//EnsureDecoratedLoggerRegistered(services, handlerType);
				HandlerRegisteringHelper.RegisterHandlerWithDecoratedLogger(services, handlerType);
			}

			var handlerTypesInAssembly2 = typesInAssembly.Where(x => x.IsAssignableToGenericType(typeof(IMessageHandler<,>)));
			foreach (var handlerType in handlerTypesInAssembly2)
			{
				//var serviceType = typeof(IMessageHandler<,>).MakeGenericType(handlerType.GetTypeArgumentsFromParent(typeof(IMessageHandler<,>)));
				//services.AddScoped(serviceType, serviceProvider => RegisterHandlerHelper.CreateHandlerWithDecoratedLogger(handlerType, serviceProvider));
				//EnsureDecoratedLoggerRegistered(services, handlerType);
				HandlerRegisteringHelper.RegisterHandlerWithDecoratedLogger(services, handlerType);

			}
		}

		return new BusClientSetupProviderStage(services);
	}
}
