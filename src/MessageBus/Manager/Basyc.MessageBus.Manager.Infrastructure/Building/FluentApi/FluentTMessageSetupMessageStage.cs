using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentTMessageSetupMessageStage<TMessage> : BuilderStageBase
{
	private readonly InProgressMessageRegistration inProgressMessage;
	private readonly InProgressGroupRegistration inProgressGroup;

	public FluentTMessageSetupMessageStage(IServiceCollection services, InProgressMessageRegistration inProgressMessage, InProgressGroupRegistration inProgressGroup) : base(services)
	{
		this.inProgressMessage = inProgressMessage;
		this.inProgressGroup = inProgressGroup;
	}

	public FluentSetupNoReturnStage NoReturn()
	{
		return new FluentSetupNoReturnStage(services, inProgressMessage, inProgressGroup);
	}

	public FluentTMessageSetupReturnStage<TMessage> Returns(Type messageResponseRuntimeType, string repsonseTypeDisplayName)
	{
		inProgressMessage.ResponseRunTimeType = messageResponseRuntimeType;
		inProgressMessage.ResponseRunTimeTypeDisplayName = repsonseTypeDisplayName;
		return new FluentTMessageSetupReturnStage<TMessage>(services, inProgressMessage, inProgressGroup);
	}

	public FluentTMessageSetupReturnStage<TMessage> Returns(Type messageResponseRuntimeType)
	{
		return Returns(messageResponseRuntimeType, messageResponseRuntimeType.Name);
	}

	public FluentTMessageTReturnSetupReturnStage<TMessage, TResponse> Returns<TResponse>()
	{
		var responseType = typeof(TResponse);
		return Returns<TResponse>(responseType.Name);
	}

	public FluentTMessageTReturnSetupReturnStage<TMessage, TResponse> Returns<TResponse>(string repsonseTypeDisplayName)
	{
		inProgressMessage.ResponseRunTimeType = typeof(TResponse);
		inProgressMessage.ResponseRunTimeTypeDisplayName = repsonseTypeDisplayName;
		return new FluentTMessageTReturnSetupReturnStage<TMessage, TResponse>(services, inProgressMessage, inProgressGroup);

	}
}
