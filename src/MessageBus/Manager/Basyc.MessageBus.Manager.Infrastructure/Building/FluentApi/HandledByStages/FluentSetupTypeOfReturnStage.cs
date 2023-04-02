using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Application;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Throw;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.HandledByStages;

public class FluentSetupTypeOfReturnStage : BuilderStageBase
{
	private readonly FluentApiGroupRegistration fluentApiGroup;
	private readonly FluentApiMessageRegistration fluentApiMessage;
	private readonly RequestToTypeBinder? binder;

	public FluentSetupTypeOfReturnStage(IServiceCollection services, FluentApiMessageRegistration fluentApiMessage,
		FluentApiGroupRegistration fluentApiGroup) : base(services)
	{
		this.fluentApiMessage = fluentApiMessage;
		this.fluentApiGroup = fluentApiGroup;

		if (fluentApiMessage.ParametersAreFromType)
			binder = new RequestToTypeBinder(fluentApiMessage.ParametersFromType.Value());
	}

	//private FluentSetupDomainPostStage HandeledBy(RequestHandlerDelegate handler)
	//{
	//	fluentApiMessage.RequestHandler = handler;
	//	return new FluentSetupDomainPostStage(services, fluentApiGroup);
	//}

	// public FluentSetupDomainPostStage HandledBy<TReturn>(Func<RequestInput, ILogger, TReturn> handler)
	// 	where TReturn : class
	// {
	// 	object? handlerWrapper(MessageRequest requestResult, ILogger logger)
	// 	{
	// 		var returnObject = handler.Invoke(requestResult.Request, logger);
	// 		returnObject.ThrowIfNull();
	// 		ReturnObjectHelper.CheckHandlerReturnType(returnObject, requestResult.Request.MessageInfo.ResponseType!);
	// 		//requestResult.Complete(returnObject);
	// 		return returnObject;
	// 	}
	//
	// 	if (services.All(x => x.ImplementationType != typeof(CommonMessageInfoProvider)))
	// 		services.AddSingleton<IMessageInfoProvider, CommonMessageInfoProvider>();
	//
	// 	services.TryAddSingleton<InMemoryRequestHandler>();
	//
	// 	services.Configure<CommonMessageInfoProviderOptions>(x =>
	// 	{
	// 		var messageRegistration = new MessageRegistration();
	// 		messageRegistration.MessageDisplayName = fluentApiMessage.MessageDisplayName;
	// 		messageRegistration.Parameters.AddRange(fluentApiMessage.Parameters);
	// 		messageRegistration.ResponseRunTimeType = fluentApiMessage.ResponseRunTimeType;
	// 		messageRegistration.ResponseRunTimeTypeDisplayName = fluentApiMessage.ResponseRunTimeTypeDisplayName;
	// 		// messageRegistration.HandlerUniqueName = InMemoryRequestHandler.InMemoryDelegateRequesterUniqueName;
	// 		messageRegistration.HandlerDelegate = handlerWrapper;
	// 		var group = x.MessageGroupRegistration.FirstOrDefault(x => x.Name == fluentApiGroup.Name);
	// 		if (group == default)
	// 		{
	// 			group = new MessageGroupRegistration(fluentApiGroup.Name.Value());
	// 			x.MessageGroupRegistration.Add(group);
	// 		}
	//
	// 		group.MessageRegistrations.Add(messageRegistration);
	// 	});
	// 	return new FluentSetupDomainPostStage(services, fluentApiGroup);
	// }

	// public FluentSetupDomainPostStage HandledBy<TReturn>(Func<RequestInput, ILogger, TReturn> handler)
	// 	where TReturn : class
	// {
	// 	object? handlerWrapper(MessageRequest requestResult, ILogger logger)
	// 	{
	// 		var returnObject = handler.Invoke(requestResult.Request, logger);
	// 		returnObject.ThrowIfNull();
	// 		ReturnObjectHelper.CheckHandlerReturnType(returnObject, requestResult.Request.MessageInfo.ResponseType!);
	// 		return returnObject;
	// 	}
	// 	ReturnStageHelper.RegisterMessageRegistration(services, fluentApiGroup, fluentApiMessage, handlerWrapper);
	// 	return new FluentSetupDomainPostStage(services, fluentApiGroup);
	// }

	public FluentSetupDomainPostStage HandledBy<TReturn>(Func<object, ILogger, TReturn> handler)
		where TReturn : class
	{
		object? handlerWrapper(MessageRequest requestResult, ILogger logger)
		{
			var message = binder.Value().CreateMessage(requestResult.Request);
			var returnObject = handler.Invoke(message, logger);
			returnObject.ThrowIfNull();
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, requestResult.Request.MessageInfo.ResponseType!);
			return returnObject;
		}

		ReturnStageHelper.RegisterMessageRegistration(services, fluentApiGroup, fluentApiMessage, handlerWrapper);
		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}

	public FluentSetupDomainPostStage HandledBy<TReturn>(Func<ReadOnlyCollection<Parameter>, ILogger, TReturn> handler)
		where TReturn : class
	{
		object? handlerWrapper(MessageRequest requestResult, ILogger logger)
		{
			var returnObject = handler.Invoke(requestResult.Request.Parameters, logger);
			returnObject.ThrowIfNull();
			ReturnObjectHelper.CheckHandlerReturnType(returnObject, requestResult.Request.MessageInfo.ResponseType!);
			return returnObject;
		}

		ReturnStageHelper.RegisterMessageRegistration(services, fluentApiGroup, fluentApiMessage, handlerWrapper);
		return new FluentSetupDomainPostStage(services, fluentApiGroup);
	}
}
