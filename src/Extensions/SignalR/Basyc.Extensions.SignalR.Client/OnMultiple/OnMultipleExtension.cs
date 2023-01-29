using Basyc.Extensions.SignalR.Client.OnMultiple;
using Microsoft.AspNetCore.SignalR.Client;
using System.Reflection;

namespace Basyc.Extensions.SignalR.Client;

public static class OnMultipleExtension
{
	private static readonly MethodInfo[] methodsToIgnore = new object().GetType().GetMethodsRecursive(BindingFlags.Public | BindingFlags.Instance);

	/// <summary>
	///     Returns subsribrion that can be closed when calling <see cref="IDisposable.Dispose" />
	/// </summary>
	/// <typeparam name="TMethodsServerCanCall"></typeparam>
	/// <param name="hubConnection"></param>
	/// <param name="serverMethods"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	public static OnMultipleSubscription OnMultiple<TMethodsServerCanCall>(HubConnection hubConnection, TMethodsServerCanCall serverMethods)
	{
		var methodInfos = FilterMethods(serverMethods);
		var innerSubsriptions = new IDisposable[methodInfos.Length];
		for (var methodIndex = 0; methodIndex < methodInfos.Length; methodIndex++)
		{
			var methodInfo = methodInfos[methodIndex];
			var parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
			if (methodInfo.ReturnType == typeof(Task))
			{
				var innerSubscription = hubConnection.On(methodInfo.Name, parameterTypes, arguments => (Task)methodInfo.Invoke(serverMethods, arguments)!);
				innerSubsriptions[methodIndex] = innerSubscription;
				continue;
			}

			if (methodInfo.ReturnType == typeof(void))
			{
				var innerSubscription = hubConnection.On(methodInfo.Name, parameterTypes, arguments =>
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

	private static MethodInfo[] FilterMethods<TMethodsServerCanCall>(TMethodsServerCanCall serverMethods)
	{
		var targetType =
			typeof(TMethodsServerCanCall); //Ignore methods that are not specified in interface (They should not be called from server + server hub does even see them)
		var filteredMethods = targetType
			.GetMethodsRecursive(BindingFlags.Public | BindingFlags.Instance)
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
}
