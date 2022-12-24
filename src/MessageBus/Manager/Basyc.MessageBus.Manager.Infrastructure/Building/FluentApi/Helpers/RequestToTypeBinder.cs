using Basyc.MessageBus.Manager.Application;
using System;
using System.Linq;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;

public class RequestToTypeBinder<TMessage>
{
	private static readonly Type messageRuntimeType;
	private static Type[] requestParameterTypes;
	private static readonly PropertyInfo[] messageClassProperties;

	static RequestToTypeBinder()
	{
		messageRuntimeType = typeof(TMessage);
		messageClassProperties = messageRuntimeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
	}

	public TMessage CreateMessage(Request request)
	{

		if (TryCreateMessageWithCtor(request, out var messageInstance))
		{
			return messageInstance!;
		}

		if (TryCreateMessageWithSetters(request, out messageInstance))
		{
			return messageInstance!;
		}

		throw new Exception("Failed to create instance of message");
	}

	public bool TryCreateMessageWithCtor(Request request, out TMessage? message)
	{
		EnsureRequestTypeParameterTypesAreCached(request);

		var promisingCtor = messageRuntimeType.GetConstructor(requestParameterTypes!);

		if (promisingCtor is null)
		{
			message = default;
			return false;
		}

		var requestParameterValues = request.Parameters.Select(x => x.Value).ToArray();
		try
		{
			var messageInstance = (TMessage)promisingCtor.Invoke(requestParameterValues);
			message = messageInstance;
			return true;
		}
		catch (Exception ex)
		{
			message = default;
			return false;
		}
	}

	private bool TryCreateMessageWithSetters(Request request, out TMessage? message)
	{
		EnsureRequestTypeParameterTypesAreCached(request);

		if (messageClassProperties.Length != requestParameterTypes!.Length)
		{
			message = default;
			return false;
		}

		if (messageClassProperties.Select(x => x.PropertyType).SequenceEqual(requestParameterTypes))
		{
			message = default;
			return false;
		}

		TMessage messageInstance = Activator.CreateInstance<TMessage>();
		for (int parameterIndex = 0; parameterIndex < requestParameterTypes.Length; parameterIndex++)
		{
			var requestParameterType = requestParameterTypes[parameterIndex];
			var messagePropertyInfo = messageClassProperties[parameterIndex];
			var requestParameter = request.Parameters.ElementAt(parameterIndex);

			messagePropertyInfo.SetValue(messageInstance, requestParameter.Value);
		}

		message = messageInstance;
		return true;
	}

	private static void EnsureRequestTypeParameterTypesAreCached(Request request)
	{
		if (requestParameterTypes is null)
		{
			requestParameterTypes = request.RequestInfo.Parameters.Select(x => x.Type).ToArray();
		}
	}
}
