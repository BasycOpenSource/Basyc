using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.AspNetCore.SignalR.Client;

namespace Basyc.Extensions.SignalR.Client;

internal class HubClientInterceptor : IInterceptor
{
    private static readonly MethodInfo[] methodsToIgnore = new object().GetType().GetMethodsRecursive(BindingFlags.Public | BindingFlags.Instance);

    private readonly Dictionary<MethodInfo, InterceptedMethodMetadata> methodInfoToMethodMetadataMap = new();

    private readonly bool usingBaseClass;

    public HubClientInterceptor(HubConnection connection, Type hubClientInterfaceType, bool usingBaseClass = false)
    {
        this.usingBaseClass = usingBaseClass;
        CreateInteceptorsForPublicMethods(connection, hubClientInterfaceType);
    }

    internal List<InterceptedMethodMetadata> InterceptedMethods { get; } = new();

    public static bool HasCancelToken(ParameterInfo[] paramInfos, out int cancelTokenParamIndex)
    {
        var cancelTokenParamInfo = paramInfos.FirstOrDefault(x => x.ParameterType == typeof(CancellationToken));
        cancelTokenParamIndex = cancelTokenParamInfo is null ? -1 : cancelTokenParamInfo.Position;

        return cancelTokenParamInfo != null;
    }

    public void Intercept(IInvocation invocation)
    {
        var methodMetadata = methodInfoToMethodMetadataMap[invocation.Method];
        var sendCoreTask = methodMetadata.SendCoreCall.Invoke(methodMetadata, invocation.Arguments);
        if (methodMetadata.ReturnsTask)
        {
            async Task Continue()
            {
                if (usingBaseClass)
                {
                    invocation.Proceed();
                }

                await sendCoreTask!;
            }

            invocation.ReturnValue = Continue();
        }
    }

    private static MethodInfo[] GetMethodsToIntercept(Type methodsClientCanCallType, bool isClass)
    {
        var publicMethods = methodsClientCanCallType.GetMethodsRecursive(BindingFlags.Instance | BindingFlags.Public);
        if (isClass is false)
        {
            return publicMethods;
        }

        return FilterObjectMethods(FilterObjectMethods(publicMethods));
    }

    private static MethodInfo[] FilterObjectMethods(MethodInfo[] methodsTofilter)
    {
        var filteredMethods = methodsTofilter
            .Where(methodInfo =>
            {
                if (methodInfo.IsSpecialName)
                {
                    return false;
                }

                return methodsToIgnore.All(methodToIgnore => methodToIgnore.Name != methodInfo.Name);
            })
            .ToArray();
        return filteredMethods;
    }

    private static CancellationToken GetCancelToken(object?[] arguments, int cancelTokenIndex) => (CancellationToken)arguments[cancelTokenIndex]!;

    private static object?[] FilterArgumentsToSend(object?[] arguments, bool hasCancelToken, int cancelTokenIndex)
    {
        if (hasCancelToken is false)
        {
            return arguments;
        }

        object?[] filteredArguments = new object?[arguments.Length - 1];
        bool cancelTokenFound = false;
        for (int argIndex = 0; argIndex < arguments.Length; argIndex++)
        {
            if (argIndex == cancelTokenIndex)
            {
                cancelTokenFound = true;
                continue;
            }

            filteredArguments[cancelTokenFound ? argIndex - 1 : argIndex] = arguments[argIndex];
        }

        return filteredArguments;
    }

    private static bool CheckAndThrowMethodSignatures(MethodInfo methodInfo, out bool returnsVoid, out bool returnsTask)
    {
        if (methodInfo.ReturnType == typeof(Task))
        {
            returnsVoid = false;
            returnsTask = true;
            return true;
        }

        if (methodInfo.ReturnType == typeof(void))
        {
            returnsVoid = true;
            returnsTask = false;
            return true;
        }

        throw new ArgumentException(
            $"Only interfaces that have only methods with return values of types {typeof(void).Name} or {nameof(Task)} can be interceted.");
    }

    private void CreateInteceptorsForPublicMethods(HubConnection connection, Type methodsClientCanCallType)
    {
        CheckAndThrowType(methodsClientCanCallType, out bool isClass);
        var methodsToIntercept = GetMethodsToIntercept(methodsClientCanCallType, isClass);
        foreach (var methodInfo in methodsToIntercept)
        {
            CheckAndThrowMethodSignatures(methodInfo, out bool returnsVoid, out bool returnsTask);
            var methodParamInfos = methodInfo.GetParameters();
            var paramTypes = methodParamInfos.Select(x => x.ParameterType).ToArray();
            bool hasCancelToken = HasCancelToken(methodParamInfos, out int cancelTokenParamIndex);

            Task? SendCoreCall(InterceptedMethodMetadata metadata, object?[] arguments)
            {
                var cancelToken = metadata.HasCancelToken ? GetCancelToken(arguments, metadata.CancelTokenIndex) : default;
                object?[] argumentsToSend = FilterArgumentsToSend(arguments, metadata.HasCancelToken, metadata.CancelTokenIndex);
                return connection.SendCoreAsync(methodInfo.Name, argumentsToSend, cancelToken);
            }

            InterceptedMethodMetadata methodMetadata = new(methodInfo,
                hasCancelToken,
                cancelTokenParamIndex,
                paramTypes,
                returnsTask,
                returnsVoid,
                SendCoreCall);
            InterceptedMethods.Add(methodMetadata);
            methodInfoToMethodMetadataMap.Add(methodInfo, methodMetadata);
        }
    }

    private void CheckAndThrowType(Type methodsClientCanCallType, out bool isClass)
    {
        isClass = false;
        if (methodsClientCanCallType.IsClass)
        {
            isClass = true;
        }

        if (usingBaseClass != isClass)
        {
            throw new ArgumentException("Settings does not match provided type");
        }
    }
}

#pragma warning disable SA1402 // File may only contain a single type
internal class HubClientInterceptor<THubClient> : HubClientInterceptor
#pragma warning restore SA1402 // File may only contain a single type
{
    public HubClientInterceptor(HubConnection connection) : base(connection, typeof(THubClient))
    {
    }
}
