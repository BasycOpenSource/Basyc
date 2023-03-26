using Basyc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentTMessageSetupMessageStage<TMessage> : BuilderStageBase
{
	private readonly FluentApiMessageRegistration fluentApiMessage;
	private readonly FluentApiGroupRegistration fluentApiGroup;

	public FluentTMessageSetupMessageStage(IServiceCollection services, FluentApiMessageRegistration fluentApiMessage, FluentApiGroupRegistration fluentApiGroup) : base(services)
	{
		this.fluentApiMessage = fluentApiMessage;
		this.fluentApiGroup = fluentApiGroup;
	}

	public FluentSetupNoReturnStage NoReturn()
	{
		return new FluentSetupNoReturnStage(services, fluentApiMessage, fluentApiGroup);
	}

	public FluentTMessageSetupReturnStage<TMessage> Returns(Type messageResponseRuntimeType, string repsonseTypeDisplayName)
	{
		fluentApiMessage.ResponseRunTimeType = messageResponseRuntimeType;
		fluentApiMessage.ResponseRunTimeTypeDisplayName = repsonseTypeDisplayName;
		return new FluentTMessageSetupReturnStage<TMessage>(services, fluentApiMessage, fluentApiGroup);
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
		fluentApiMessage.ResponseRunTimeType = typeof(TResponse);
		fluentApiMessage.ResponseRunTimeTypeDisplayName = repsonseTypeDisplayName;
		return new FluentTMessageTReturnSetupReturnStage<TMessage, TResponse>(services, fluentApiMessage, fluentApiGroup);

	}
}
