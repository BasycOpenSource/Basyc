using Basyc.DependencyInjection;
using Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.HandledByStages;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ParameterInfo = Basyc.MessageBus.Manager.Application.Initialization.ParameterInfo;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi;

public class FluentSetupMessageStage : BuilderStageBase
{
	private readonly FluentApiGroupRegistration fluentApiGroup;
	private readonly FluentApiMessageRegistration fluentApiMessage;

	public FluentSetupMessageStage(IServiceCollection services, FluentApiMessageRegistration fluentApiMessage, FluentApiGroupRegistration fluentApiGroup) : base(services)
	{
		this.fluentApiMessage = fluentApiMessage;
		this.fluentApiGroup = fluentApiGroup;
	}

	public FluentSetupMessageStage WithParameter<TParameter>(string parameterDisplayName)
	{
		fluentApiMessage.Parameters.Add(new ParameterInfo(typeof(TParameter), parameterDisplayName, typeof(TParameter).Name));
		return new FluentSetupMessageStage(services, fluentApiMessage, fluentApiGroup);
	}

	/// <summary>
	///     Registeres <typeparamref name="TMessage" /> public properties as message parameters
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	public FluentTMessageSetupMessageStage<TMessage> WithParametersFrom<TMessage>()
	{
		foreach (var parameter in typeof(TMessage).GetProperties(BindingFlags.Instance | BindingFlags.Public))
			fluentApiMessage.Parameters.Add(new ParameterInfo(parameter.PropertyType, parameter.Name, parameter.PropertyType.Name));
		fluentApiMessage.ParametersFromType = typeof(TMessage);
		return new FluentTMessageSetupMessageStage<TMessage>(services, fluentApiMessage, fluentApiGroup);
	}

	public FluentSetupMessageStage WithParametersFrom(Type type)
	{
		foreach (var parameter in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			fluentApiMessage.Parameters.Add(new ParameterInfo(parameter.PropertyType, parameter.Name, parameter.PropertyType.Name));
		fluentApiMessage.ParametersFromType = type;
		//return new FluentTMessageSetupMessageStage<object>(services, fluentApiMessage, fluentApiGroup);
		return new FluentSetupMessageStage(services, fluentApiMessage, fluentApiGroup);
	}

	public FluentSetupNoReturnHandledByStage NoReturn()
	{
		return new FluentSetupNoReturnHandledByStage(services, fluentApiMessage, fluentApiGroup);
	}

	public FluentSetupTypeOfReturnStage Returns(Type messageResponseRuntimeType, string repsonseTypeDisplayName)
	{
		fluentApiMessage.ResponseRunTimeType = messageResponseRuntimeType;
		fluentApiMessage.ResponseRunTimeTypeDisplayName = repsonseTypeDisplayName;
		return new FluentSetupTypeOfReturnStage(services, fluentApiMessage, fluentApiGroup);
	}

	public FluentSetupTypeOfReturnStage Returns(Type messageResponseRuntimeType)
	{
		return Returns(messageResponseRuntimeType, messageResponseRuntimeType.Name);
	}

	public FluentSetupTypeOfReturnStage Returns<TReponse>()
	{
		var responseType = typeof(TReponse);
		return Returns(responseType);
	}

	public FluentSetupTypeOfReturnStage Returns<TReponse>(string repsonseTypeDisplayName)
	{
		var responseType = typeof(TReponse);
		return Returns(responseType, repsonseTypeDisplayName);
	}
}
