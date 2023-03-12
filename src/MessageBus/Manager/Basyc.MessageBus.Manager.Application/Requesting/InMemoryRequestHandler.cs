using Basyc.Diagnostics.Shared;
using Basyc.MessageBus.Manager.Application.Building;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class InMemoryRequestHandler : IRequestHandler
{
	public const string InMemoryDelegateRequesterUniqueName = nameof(InMemoryRequestHandler);
	public static int ctorCounter;

	private readonly Dictionary<MessageInfo, Action<MessageRequest>> handlersMap;
	private readonly IOptions<InMemoryRequestHandlerOptions> options;

	public InMemoryRequestHandler(IOptions<InMemoryRequestHandlerOptions> options)
	{
		this.options = options;
		handlersMap = options.Value.ResolveHandlers();
		ctorCounter++;
	}

	public string UniqueName => InMemoryDelegateRequesterUniqueName;

	public void StartRequest(MessageRequest requestResult, ILogger logger)
	{
		logger.LogInformation("Starting invoking in-memory delegate");
		// var handler = handlersMap[requestResult.Request.RequestInfo];
		using var findingHandlerActivity = DiagnosticHelper.Start("Finding handler");
		var handlerFound = handlersMap.TryGetValue(requestResult.Request.MessageInfo, out var handler);
		if (handlerFound is false)
		{
			requestResult.Fail(
				$"Requester: '{nameof(InMemoryRequestHandler)}' doesn't have handler for message with display name: '{requestResult.Request.MessageInfo.RequestDisplayName}'");
			findingHandlerActivity.Stop();
			return;
		}

		findingHandlerActivity.Stop();
		var waitingForTaskRun = DiagnosticHelper.Start("Waiting for Task.Run");
		Task.Run(() =>
		{
			waitingForTaskRun.Stop();
			using var startActivity = requestResult.StartDurationMap();
			using var invokingHandlerActivity = DiagnosticHelper.Start("Invoking handler", startActivity.TraceId, startActivity.Id);
			handler.Value().Invoke(requestResult);
			logger.LogInformation("Handler finished");
			invokingHandlerActivity.Stop();
			requestResult.FinishDurationMap();
		}).ContinueWith(x =>
		{
			if (x.Status is TaskStatus.Faulted)
			{
				logger.LogError("Handler failed");
				requestResult.Fail(x.Exception.Value().Message);
			}
		});
	}

	public void AddHandler(MessageInfo requestInfo, Action<MessageRequest> handler)
	{
		handlersMap.Add(requestInfo, handler);
	}
}
