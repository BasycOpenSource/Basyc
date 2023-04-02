using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.HandledByStages;
using Microsoft.Extensions.DependencyInjection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentTMessageSetupMessageStage<TMessage> : BuilderStageBase
{
	private readonly FluentApiGroupRegistration fluentApiGroup;
	private readonly FluentApiMessageRegistration fluentApiMessage;

	public FluentTMessageSetupMessageStage(IServiceCollection services, FluentApiMessageRegistration fluentApiMessage, FluentApiGroupRegistration fluentApiGroup) :
		base(services)
	{
		this.fluentApiMessage = fluentApiMessage;
		this.fluentApiGroup = fluentApiGroup;
	}

	public FluentSetupNoReturnHandledByStage NoReturn()
	{
		return new FluentSetupNoReturnHandledByStage(services, fluentApiMessage, fluentApiGroup);
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
