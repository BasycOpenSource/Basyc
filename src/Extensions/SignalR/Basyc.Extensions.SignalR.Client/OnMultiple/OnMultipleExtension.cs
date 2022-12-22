using Basyc.Extensions.SignalR.Client.OnMultiple;
using Microsoft.AspNetCore.SignalR.Client;
using System.Reflection;

namespace Basyc.Extensions.SignalR.Client;

public static class OnMultipleExtension
{
	/// <summary>
	/// Returns subsribrion that can be closed when calling <see cref="IDisposable.Dispose"/>
	/// </summary>
	/// <typeparam name="TMethodsServerCanCall"></typeparam>
	/// <param name="hubConnection"></param>
	/// <param name="serverMethods"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	public static OnMultipleSubscription OnMultiple<TMethodsServerCanCall>(HubConnection hubConnection, TMethodsServerCanCall serverMethods)
	{
		var methodInfos = FilterMethods(serverMethods);
		IDisposable[] innerSubsriptions = new IDisposable[methodInfos.Length];
		for (int methodIndex = 0; methodIndex < methodInfos.Length; methodIndex++)
		{
			MethodInfo? methodInfo = methodInfos[methodIndex];
			Type[] parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
			if (methodInfo.ReturnType == typeof(Task))
			{
				var innerSubscription = hubConnection.On(methodInfo.Name, parameterTypes, (arguments) => (Task)methodInfo.Invoke(serverMethods, arguments)!);
				innerSubsriptions[methodIndex] = innerSubscription;
				continue;
			}

			if (methodInfo.ReturnType == typeof(void))
			{
				var innerSubscription = hubConnection.On(methodInfo.Name, parameterTypes, (arguments) =>
				{
					methodInfo.Invoke(serverMethods, arguments);
					return Task.CompletedTask;
				});
				innerSubsriptions[methodIndex] = innerSubscription;
				continue;
			}

			throw new ArgumentException("Class must not contain public methods with different return types than void and Task");
		}

		return new OnMultipleSubscription(innerSubsriptions);
	}

	private static readonly MethodInfo[] MethodsToIgnore = new object().GetType().GetMethodsRecursive(BindingFlags.Public | BindingFlags.Instance);

	private static MethodInfo[] FilterMethods<TMethodsServerCanCall>(TMethodsServerCanCall serverMethods)
	{
		Type targetType = typeof(TMethodsServerCanCall); //Ignore methods that are not specified in interface (They should not be called from server + server hub does even see them)
		var filteredMethods = targetType
			.GetMethodsRecursive(BindingFlags.Public | BindingFlags.Instance)
			.Where<MethodInfo>(methodInfo =>
			{
				if (methodInfo.IsSpecialName)
					return false;
				return MethodsToIgnore.All(methodToIgnore => methodToIgnore.Name != methodInfo.Name);
			})
			.ToArray();

		return filteredMethods;

	}
}