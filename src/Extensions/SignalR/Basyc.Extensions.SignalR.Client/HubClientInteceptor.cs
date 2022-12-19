using Castle.DynamicProxy;
using Microsoft.AspNetCore.SignalR.Client;
using System.Reflection;

namespace Basyc.Extensions.SignalR.Client
{
    internal class HubClientInteceptor : IInterceptor
    {
        internal List<InterceptedMethodMetadata> InterceptedMethods { get; } = new();
        private readonly Dictionary<MethodInfo, InterceptedMethodMetadata> methodInfoToMethodMetadataMap = new();

        public HubClientInteceptor(HubConnection connection, Type hubClientInterfaceType, bool usingBaseClass = false)
        {
            this.usingBaseClass = usingBaseClass;
            CreateInteceptorsForPublicMethods(connection, hubClientInterfaceType);
        }

        public void Intercept(IInvocation invocation)
        {
            var methodMetadata = methodInfoToMethodMetadataMap[invocation.Method];
            var sendCoreTask = methodMetadata.SendCoreCall.Invoke(methodMetadata, invocation.Arguments);
            if (methodMetadata.ReturnsTask)
            {
                Func<Task> continuation = async () =>
                {
                    if (usingBaseClass)
                    {
                        invocation.Proceed();
                    }
                    await sendCoreTask!;
                };
                invocation.ReturnValue = continuation();
            }
        }
        private void CreateInteceptorsForPublicMethods(HubConnection connection, Type methodsClientCanCallType)
        {
            CheckAndThrowType(methodsClientCanCallType, out var isClass);
            MethodInfo[] methodsToIntercept = GetMethodsToIntercept(methodsClientCanCallType, isClass);
            foreach (var methodInfo in methodsToIntercept)
            {
                CheckAndThrowMethodSignatures(methodInfo, out var returnsVoid, out var returnsTask);
                ParameterInfo[] methodParamInfos = methodInfo.GetParameters();
                var paramTypes = methodParamInfos.Select(x => x.ParameterType).ToArray();
                var hasCancelToken = HasCancelToken(methodParamInfos, out var cancelTokenParamIndex);
                Func<InterceptedMethodMetadata, object?[], Task?> sendCoreCall = (metadata, arguments) =>
                {
                    CancellationToken cancelToken = metadata.HasCancelToken ? GetCancelToken(arguments, metadata.CancelTokenIndex) : default;
                    var argumentsToSend = FilterArgumentsToSend(arguments, metadata.HasCancelToken, metadata.CancelTokenIndex);
                    return connection.SendCoreAsync(methodInfo.Name, argumentsToSend, cancelToken);
                };
                InterceptedMethodMetadata methodMetadata = new(methodInfo,
                                                   hasCancelToken,
                                                   cancelTokenParamIndex,
                                                   paramTypes,
                                                   returnsTask,
                                                   returnsVoid,
                                                   sendCoreCall);
                InterceptedMethods.Add(methodMetadata);
                methodInfoToMethodMetadataMap.Add(methodInfo, methodMetadata);
            }
        }

        private static MethodInfo[] GetMethodsToIntercept(Type methodsClientCanCallType, bool isClass)
        {
            var publicMethods = methodsClientCanCallType.GetMethodsRecursive(BindingFlags.Instance | BindingFlags.Public);
            if (isClass is false)
            {
                return publicMethods;
            }
            else
            {
                return FilterObjectMethods(FilterObjectMethods(publicMethods));
            }

        }

        private static readonly MethodInfo[] MethodsToIgnore = new object().GetType().GetMethodsRecursive(BindingFlags.Public | BindingFlags.Instance);
        private readonly bool usingBaseClass;

        private static MethodInfo[] FilterObjectMethods(MethodInfo[] methodsTofilter)
        {
            var filteredMethods = methodsTofilter
                .Where(methodInfo =>
                {
                    if (methodInfo.IsSpecialName)
                        return false;
                    return MethodsToIgnore.All(methodToIgnore => methodToIgnore.Name != methodInfo.Name);
                })
                .ToArray();
            return filteredMethods;
        }

        private void CheckAndThrowType(Type methodsClientCanCallType, out bool isClass)
        {
            isClass = false;
            if (methodsClientCanCallType.IsClass)
            {

                isClass = true;
            }

            if (usingBaseClass != isClass)
                throw new ArgumentException("Settings does not match provided type");
        }

        public bool HasCancelToken(ParameterInfo[] paramInfos, out int cancelTokenParamIndex)
        {
            var cancelTokenParamInfo = paramInfos.FirstOrDefault(x => x.ParameterType == typeof(CancellationToken));
            if (cancelTokenParamInfo is null)
                cancelTokenParamIndex = -1;
            else
                cancelTokenParamIndex = cancelTokenParamInfo.Position;
            return cancelTokenParamInfo != null;
        }



        private static CancellationToken GetCancelToken(object?[] arguments, int cancelTokenIndex)
        {
            return (CancellationToken)arguments[cancelTokenIndex]!;
        }

        private static object?[] FilterArgumentsToSend(object?[] arguments, bool hasCancelToken, int cancelTokenIndex)
        {
            if (hasCancelToken is false)
                return arguments;

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

            throw new ArgumentException($"Only interfaces that have only methods with return values of types {typeof(void).Name} or {typeof(Task).Name} can be interceted.");
        }
    }
    internal class HubClientInteceptor<THubClient> : HubClientInteceptor
    {
        public HubClientInteceptor(HubConnection connection) : base(connection, typeof(THubClient))
        {
        }
    }


}