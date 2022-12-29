using Basyc.Diagnostics.Shared;
using Basyc.MessageBus.Client.Diagnostics;
using Basyc.MessageBus.Client.RequestResponse;
using Basyc.MessageBus.NetMQ.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OneOf;

namespace Basyc.MessageBus.Client.NetMQ;

public class MessageHandlerManager : IMessageHandlerManager
{
	private record HandlerMetadata(NetMQMessageHandlerInfo HandlerInfo, Type HandlerRuntimeType);
	private readonly IServiceProvider serviceProvider;
	private readonly IOptions<MessageHandlerManagerOptions> options;
	private readonly Dictionary<string, HandlerMetadata> handlerTypesCacheMap = new();
	public MessageHandlerManager(IServiceProvider serviceProvider, IOptions<MessageHandlerManagerOptions> options)
	{
		this.serviceProvider = serviceProvider;
		this.options = options;

		foreach (var handlerInfo in options.Value.HandlerInfos)
		{
			Type handlerInterfaceType;
			Type handlerType;

			if (handlerInfo.HasResponse)
			{
				handlerInterfaceType = typeof(IMessageHandler<,>);
				handlerType = handlerInterfaceType.MakeGenericType(handlerInfo.MessageType, handlerInfo.ResponseType!);
			}
			else
			{
				handlerInterfaceType = typeof(IMessageHandler<>);
				handlerType = handlerInterfaceType.MakeGenericType(handlerInfo.MessageType);
			}

			HandlerMetadata newHandlerMetadata = new(handlerInfo, handlerType);
			handlerTypesCacheMap.Add(handlerInfo.MessageSimpleType, newHandlerMetadata);
		}
	}

	public async Task<OneOf<object, Exception>> ConsumeMessage(string messageType, object? messageData, CancellationToken cancellationToken, string traceId, string parentSpanId)
	{
		if (handlerTypesCacheMap.TryGetValue(messageType, out var handlerMetadata) is false)
			throw new InvalidOperationException("Handler for this message not found");

		object handler = serviceProvider.GetRequiredService(handlerMetadata.HandlerRuntimeType)!;
		BusHandlerLoggerSessionManager.StartSession(new LoggingSession(traceId, handlerMetadata.HandlerInfo.HandleMethodInfo.Name));

		//var activityTraceId = ActivityTraceId.CreateFromString(traceId);
		//var activitySpanId = ActivitySpanId.CreateFromString(parentSpanId);
		//var activityContext = new ActivityContext(activityTraceId, activitySpanId, ActivityTraceFlags.Recorded, null, true);

		//using (var handlerStartedActivity = DiagnosticConstants.HandlerStarted.StartActivity("ConsumeMessage", ActivityKind.Internal, activityContext, new KeyValuePair<string, object?>[]
		//{
		//	new KeyValuePair<string, object?>(DiagnosticConstants.ShouldBeReceived ,true)
		//}))
		using var handlerStartedActivity = DiagnosticHelper.Start("ConsumeMessage");
		//if (handlerStartedActivity is not null)
		//{
		//	handlerStartedActivity.AddBaggage(DiagnosticConstants.ShouldBeReceived, true.ToString());
		//}

		var invokeActivity = DiagnosticHelper.Start("Invoking method info");
		Task handlerResultTask = (Task)handlerMetadata.HandlerInfo.HandleMethodInfo.Invoke(handler, new object[] { messageData!, cancellationToken })!;
		object? handlerResult;
		try
		{
			if (handlerMetadata.HandlerInfo.HasResponse)
			{
				object taskResult = ((dynamic)handlerResultTask).Result!;
				invokeActivity.Stop();
				handlerResult = taskResult;
			}
			else
			{
				await handlerResultTask;
				invokeActivity.Stop();
				handlerResult = new VoidResult();
			}
		}
		catch (Exception ex)
		{
			var endSessionActivity2 = DiagnosticHelper.Start("Basyc.MessageBus.Client.NetMQ.MessageHandlerManager End BusHandlerLoggerSessionManager session");
			BusHandlerLoggerSessionManager.EndSession();
			endSessionActivity2.Stop();
			invokeActivity.Stop();
			return ex;
		}

		var endSessionActivity = DiagnosticHelper.Start("Basyc.MessageBus.Client.NetMQ.MessageHandlerManager End BusHandlerLoggerSessionManager session");
		BusHandlerLoggerSessionManager.EndSession();
		endSessionActivity.Stop();

		return handlerResult;
	}

	public string[] GetConsumableMessageTypes()
	{
		return handlerTypesCacheMap.Values
			.Select(handlerMetadata => handlerMetadata.HandlerInfo.MessageSimpleType)
			.ToArray();
	}
}
