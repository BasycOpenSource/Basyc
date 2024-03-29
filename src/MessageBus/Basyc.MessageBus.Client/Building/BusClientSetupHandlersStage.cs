﻿using Basyc.DependencyInjection;
using Basyc.MessageBus.Client.Diagnostics;
using Basyc.MessageBus.Client.RequestResponse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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

	public BusClientSetupProviderStage RegisterBasycTypedHandlers<THandlerAssemblyMarker>()
	{
		return RegisterBasycTypedHandlers(typeof(THandlerAssemblyMarker).Assembly);
	}

	public BusClientSetupProviderStage RegisterBasycTypedHandlers(params Assembly[] assembliesToScan)
	{

		foreach (var assembly in assembliesToScan)
		{
			Type[] typesInAssembly = assembly.GetTypes();
			var handlerTypesInAssembly = typesInAssembly.Where(x => x.IsAssignableToGenericType(typeof(IMessageHandler<>)));
			foreach (var handlerType in handlerTypesInAssembly)
			{
				var serviceType = typeof(IMessageHandler<>).MakeGenericType(GenericsHelper.GetTypeArgumentsFromParent(handlerType, typeof(IMessageHandler<>)));
				services.AddScoped(serviceType, serviceProvider => CreateHandlerWithDecoratedLoggerT(handlerType, serviceProvider));
				EnsureHandlerLoggerRegistered(services, handlerType);

			}

			var handlerTypesInAssembly2 = typesInAssembly.Where(x => x.IsAssignableToGenericType(typeof(IMessageHandler<,>)));
			foreach (var handlerType in handlerTypesInAssembly2)
			{
				var serviceType = typeof(IMessageHandler<,>).MakeGenericType(GenericsHelper.GetTypeArgumentsFromParent(handlerType, typeof(IMessageHandler<,>)));
				services.AddScoped(serviceType, serviceProvider => CreateHandlerWithDecoratedLoggerT(handlerType, serviceProvider));
				EnsureHandlerLoggerRegistered(services, handlerType);

			}
		}

		return new BusClientSetupProviderStage(services);
	}

	private static object CreateHandlerWithDecoratedLoggerT(Type handlerType, IServiceProvider services)
	{
		ConstructorInfo handlerConstrucor = getHandlerConstructor(handlerType);

		var ctorParams = handlerConstrucor.GetParameters();
		object[] ctorArguments = new object[ctorParams.Length];
		for (int paramIndex = 0; paramIndex < ctorParams.Length; paramIndex++)
		{
			ParameterInfo ctorParam = ctorParams[paramIndex];
			if (ctorParam.ParameterType == typeof(ILogger))
			{
				var handlerLogger = services.GetRequiredService(typeof(BusHandlerLogger));
				ctorArguments[paramIndex] = handlerLogger;
				continue;
			}

			if (ctorParam.ParameterType.IsAssignableToGenericType(typeof(ILogger<>)))
			{
				var originalLoggerGenericArgument = ctorParam.ParameterType.GetTypeArgumentsFromParent(typeof(ILogger<>))[0];
				var decoLoggerType = typeof(BusHandlerLogger<>).MakeGenericType(originalLoggerGenericArgument);
				var decoLogger = services.GetRequiredService(decoLoggerType);
				ctorArguments[paramIndex] = decoLogger;
				continue;
			}

			ctorArguments[paramIndex] = services.GetRequiredService(ctorParam.ParameterType);
		}

		var handlerInstance = handlerConstrucor.Invoke(ctorArguments);
		return handlerInstance;
	}

	private static ConstructorInfo getHandlerConstructor(Type handlerType)
	{
		var handlerConstructors = handlerType.GetConstructors();
		if (handlerConstructors.Length > 1)
			throw new Exception("Multiple contructors not supported");
		ConstructorInfo handlerConstrucor = handlerConstructors[0];
		return handlerConstrucor;
	}

	private static void EnsureHandlerLoggerRegistered(IServiceCollection services, Type originalHandlerType)
	{

		var ctor = getHandlerConstructor(originalHandlerType);
		var ctorParams = ctor.GetParameters();
		var loggerParam = ctorParams.FirstOrDefault(x => x.ParameterType == typeof(ILogger));
		if (loggerParam is null)
		{
			loggerParam = ctorParams.FirstOrDefault(x => x.ParameterType.IsAssignableToGenericType(typeof(ILogger<>)));
			if (loggerParam is null)
				return; //Handler does not have logger, we can skip
			var loggerCategory = loggerParam.ParameterType.GetTypeArgumentsFromParent(typeof(ILogger<>))[0];

			var busLoggerType = typeof(BusHandlerLogger<>).MakeGenericType(loggerCategory);
			services.TryAddSingleton(busLoggerType);
		}
		else
		{
			services.TryAddSingleton(typeof(BusHandlerLogger));
		}
	}
}
