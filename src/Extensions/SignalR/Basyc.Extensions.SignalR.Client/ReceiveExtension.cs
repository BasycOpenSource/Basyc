using System.Linq.Expressions;
using System.Reflection;

namespace Basyc.Extensions.SignalR.Client;

public static class ReceiveExtension
{
	private static readonly Dictionary<(IStrongTypedHubConnectionBase connection, object id), IDisposable> actionToSubscriptionMap = new();
	public static Task Receive<TServerCanCall, TMethodToWait>(IStrongTypedHubConnectionReceiver<TServerCanCall> strongTypedHubConnection, Expression<Func<TServerCanCall, TMethodToWait>> methodSelector, TMethodToWait handler)
		where TServerCanCall : notnull
		where TMethodToWait : Delegate
	{
		var unsubscribeAction = GetUnsubscribeAction;
		var methodInfo = GetMethodFromSelector(methodSelector);
		var taskCompletionSource = new TaskCompletionSource();
		Type[] parameterTypes = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();

		var subscription = strongTypedHubConnection.UnderlyingHubConnection.On(
		methodInfo.Name,
		parameterTypes,
		(arguments, state) =>
		{
			var unsubscribeAction = (Action<IStrongTypedHubConnectionBase, object>)state;
			unsubscribeAction.Invoke(strongTypedHubConnection, state);
			InvokeDelegate(handler, arguments);
			taskCompletionSource.SetResult();
			return Task.CompletedTask;
		},
		unsubscribeAction);
		actionToSubscriptionMap.Add((strongTypedHubConnection, unsubscribeAction), subscription);
		return taskCompletionSource.Task;
	}

	private static void GetUnsubscribeAction(IStrongTypedHubConnectionBase connection, object id)
	{
		var subscriptionId = (connection, id);
		actionToSubscriptionMap[subscriptionId].Dispose();
		actionToSubscriptionMap.Remove(subscriptionId);
	}

	private static MethodInfo GetMethodFromSelector<TServerCanCall, TMethodToWait>(Expression<Func<TServerCanCall, TMethodToWait>> methodSelector) where TServerCanCall : notnull
	{
		//https://stackoverflow.com/questions/8225302/get-the-name-of-a-method-using-an-expression
		//var memberExpression = methodSelector.Body as MemberExpression;
		//if (memberExpression == null)
		//	throw new ArgumentException();
		//var methodInfo = memberExpression.Member as MethodInfo;
		//if (methodInfo == null)
		//	throw new ArgumentException();
		//return methodInfo;

		var unaryExpression = (UnaryExpression)methodSelector.Body;
		var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
		var methodCallObject = (ConstantExpression)methodCallExpression.Object!;
		var methodInfo = (MethodInfo)methodCallObject.Value!;
		return methodInfo;
	}

	private static void InvokeDelegate<TLambda>(TLambda lambda, params object?[] arguments)
	  where TLambda : Delegate
	{
		CheckArgumentTypes(lambda, arguments);

		if (lambda.Method.ReturnType == typeof(void))
		{
			if (lambda.Method.GetParameters().Length != arguments.Length)
			{
				throw new Exception("Wrong number of arguments");
			}

			lambda.DynamicInvoke(arguments);
			return;
		}

		if (lambda.Method.ReturnType != typeof(void))
		{

			lambda.DynamicInvoke(arguments);
			return;
		}

		throw new Exception("Unknown delegate");
	}

	private static void CheckArgumentTypes<TLambda>(TLambda lambda, object?[] arguments)
		where TLambda : Delegate
	{
		ParameterInfo[] lambdaParamInfos = lambda.Method.GetParameters();

		if (lambdaParamInfos.Length != arguments.Length)
		{
			throw new Exception("Wrong number of arguments");
		}

		for (int paramIndex = 0; paramIndex < lambdaParamInfos.Length; paramIndex++)
		{
			ParameterInfo lambdaParamInfo = lambdaParamInfos[paramIndex];
			var argument = arguments[paramIndex];
			if (argument is null)
			{
				if (lambdaParamInfo.ParameterType.CanBeNull() is false)
				{
					throw new Exception("argument cant be null");
				}
			}
			else
			{
				var argType = argument.GetType();
				if (lambdaParamInfo.ParameterType.IsAssignableFrom(argType) is false)
				{
					throw new Exception($"Provided argument on index {paramIndex} does not match delegate paramter {argType.Name}");
				}
			}
		}
	}
}