using Basyc.MessageBus.Manager.Application;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;

public class RequestToTypeBinder
{
	protected static Type[]? requestParameterTypes;
	protected readonly PropertyInfo[] messageClassProperties;
	protected readonly Type messageRuntimeType;

	public RequestToTypeBinder(Type messageRuntimeType)
	{
		this.messageRuntimeType = messageRuntimeType;
		messageClassProperties = messageRuntimeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
	}

	public object CreateMessage(RequestInput request)
	{
		var message = TryCreateMessageWithCtor(request, out var messageInstance)
			? messageInstance
			: TryCreateMessageWithSetters(request, out messageInstance)
				? messageInstance
				: throw new Exception("Failed to create instance of message");

		return message;
	}

	protected bool TryCreateMessageWithCtor(RequestInput request, [NotNullWhen(true)] out object? message)
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
			var messageInstance = promisingCtor.Invoke(requestParameterValues);
			message = messageInstance;
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
			return true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
		}
		catch (Exception)
		{
			message = default;
			return false;
		}
	}

	protected bool TryCreateMessageWithSetters(RequestInput request, [NotNullWhen(true)] out object? message)
	{
		EnsureRequestTypeParameterTypesAreCached(request);

		if (messageClassProperties.Length != requestParameterTypes!.Length)
		{
			message = default;
			return false;
		}

		if (messageClassProperties.Select(x => x.PropertyType).SequenceEqual(requestParameterTypes) is false)
		{
			message = default;
			return false;
		}

		var messageInstance = Activator.CreateInstance(messageRuntimeType);
		for (var parameterIndex = 0; parameterIndex < requestParameterTypes.Length; parameterIndex++)
		{
			var messagePropertyInfo = messageClassProperties[parameterIndex];
			var requestParameter = request.Parameters.ElementAt(parameterIndex);

			messagePropertyInfo.SetValue(messageInstance, requestParameter.Value);
		}

		message = messageInstance;
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
		return true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
	}

	private static void EnsureRequestTypeParameterTypesAreCached(RequestInput request)
	{
		requestParameterTypes ??= request.MessageInfo.Parameters.Select(x => x.Type).ToArray();
	}
}

public class RequestToTypeBinder<TMessage> : RequestToTypeBinder
{
	public RequestToTypeBinder() : base(typeof(TMessage))
	{
	}

	public new TMessage CreateMessage(RequestInput request)
	{
		var message = TryCreateMessageWithCtor(request, out var messageInstance)
			? messageInstance
			: TryCreateMessageWithSetters(request, out messageInstance)
				? messageInstance
				: throw new Exception("Failed to create instance of message");

		return (TMessage)message;
	}
}
