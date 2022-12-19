using System.Reflection;

namespace Basyc.Extensions.SignalR.Client
{
	internal record InterceptedMethodMetadata(MethodInfo MethodInfo, bool HasCancelToken, int CancelTokenIndex, Type[] Parameters, bool ReturnsTask, bool ReturnsVoid, Func<InterceptedMethodMetadata, object?[], Task?> SendCoreCall);
}