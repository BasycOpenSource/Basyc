using Basyc.MessageBus.Manager.Application;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Basyc.MessageBus.Manager.Infrastructure.Building.FluentApi.Helpers;

#pragma warning disable CA1819 // Properties should not return arrays

public class RequestToTypeBinder
{
    public RequestToTypeBinder(Type messageRuntimeType)
    {
        MessageRuntimeType = messageRuntimeType;
        MessageClassProperties = messageRuntimeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    protected static Type[]? RequestParameterTypes { get; private set; }

    protected PropertyInfo[] MessageClassProperties { get; private set; }

    protected Type MessageRuntimeType { get; private set; }

    public object CreateMessage(RequestInput request)
    {
        object message = TryCreateMessageWithCtor(request, out object? messageInstance)
            ? messageInstance
            : TryCreateMessageWithSetters(request, out messageInstance)
                ? messageInstance
                : throw new InvalidOperationException("Failed to create instance of message");

        return message;
    }

    protected bool TryCreateMessageWithCtor(RequestInput request, [NotNullWhen(true)] out object? message)
    {
        EnsureRequestTypeParameterTypesAreCached(request);

        var promisingCtor = MessageRuntimeType.GetConstructor(RequestParameterTypes!);

        if (promisingCtor is null)
        {
            message = default;
            return false;
        }

        object?[] requestParameterValues = request.Parameters.Select(x => x.Value).ToArray();
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            object messageInstance = promisingCtor.Invoke(requestParameterValues);
            message = messageInstance;
            return true;
        }
        catch (Exception)
        {
            message = default;
            return false;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    protected bool TryCreateMessageWithSetters(RequestInput request, [NotNullWhen(true)] out object? message)
    {
        EnsureRequestTypeParameterTypesAreCached(request);

        if (MessageClassProperties.Length != RequestParameterTypes!.Length)
        {
            message = default;
            return false;
        }

        if (MessageClassProperties.Select(x => x.PropertyType).SequenceEqual(RequestParameterTypes) is false)
        {
            message = default;
            return false;
        }

        object? messageInstance = Activator.CreateInstance(MessageRuntimeType);
        for (int parameterIndex = 0; parameterIndex < RequestParameterTypes.Length; parameterIndex++)
        {
            var messagePropertyInfo = MessageClassProperties[parameterIndex];
            var requestParameter = request.Parameters.ElementAt(parameterIndex);

            messagePropertyInfo.SetValue(messageInstance, requestParameter.Value);
        }

        message = messageInstance;
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
        return true;
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
    }

    private static void EnsureRequestTypeParameterTypesAreCached(RequestInput request) => RequestParameterTypes ??= request.MessageInfo.Parameters.Select(x => x.Type).ToArray();
}

#pragma warning disable SA1402
public class RequestToTypeBinder<TMessage> : RequestToTypeBinder
#pragma warning restore SA1402
{
    public RequestToTypeBinder() : base(typeof(TMessage))
    {
    }

    public new TMessage CreateMessage(RequestInput request)
    {
        object message = TryCreateMessageWithCtor(request, out object? messageInstance)
            ? messageInstance
            : TryCreateMessageWithSetters(request, out messageInstance)
                ? messageInstance
                : throw new InvalidOperationException("Failed to create instance of message");

        return (TMessage)message;
    }
}
