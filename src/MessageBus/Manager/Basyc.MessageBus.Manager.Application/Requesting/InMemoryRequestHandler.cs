using Basyc.Diagnostics.Shared;
using Basyc.MessageBus.Manager.Application.Initialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class InMemoryRequestHandler : IRequestHandler
{
	public const string InMemoryDelegateRequesterUniqueName = nameof(InMemoryRequestHandler);
	public static int ctorCounter;

	private readonly Dictionary<RequestInfo, Action<RequestContext>> handlersMap;
	private readonly IOptions<InMemoryRequestHandlerOptions> options;


	public InMemoryRequestHandler(IOptions<InMemoryRequestHandlerOptions> options)
	{
		this.options = options;
		handlersMap = options.Value.ResolveHandlers();
		ctorCounter++;
	}

	public string UniqueName => InMemoryDelegateRequesterUniqueName;


	public void StartRequest(RequestContext requestResult, ILogger logger)
	{
		logger.LogInformation("Starting invoking in-memory delegate");
		// var handler = handlersMap[requestResult.Request.RequestInfo];
		using var findingHandlerActivity = DiagnosticHelper.Start("Finding handler");
		var handlerFound = handlersMap.TryGetValue(requestResult.Request.RequestInfo, out var handler);
		if (handlerFound is false)
		{
			// throw new InvalidOperationException(
			//	$"Requester: '{nameof(InMemoryRequestHandler)}' doesn't have handler for message with display name: '{requestResult.Request.RequestInfo.RequestDisplayName}'");
			requestResult.Fail(
				$"Requester: '{nameof(InMemoryRequestHandler)}' doesn't have handler for message with display name: '{requestResult.Request.RequestInfo.RequestDisplayName}'");
			findingHandlerActivity.Stop();
			return;
		}

		findingHandlerActivity.Stop();

		var waitingForTaskRun = DiagnosticHelper.Start("Waiting for Task.Run");
		Task.Run(() =>
		{
			waitingForTaskRun.Stop();
			using var invokingHandlerActivity = DiagnosticHelper.Start("Invoking handler");
			handler.Value().Invoke(requestResult);
			invokingHandlerActivity.Stop();
			logger.LogInformation("In-memory delegate completed");
		}).ContinueWith(x =>
		{
			if (x.Status is TaskStatus.Faulted)
			{
				logger.LogError("In-memory delegate failed");
				requestResult.Fail(x.Exception.Value().Message);
			}
		});
	}

	public void AddHandler(RequestInfo requestInfo, Action<RequestContext> handler)
	{
		handlersMap.Add(requestInfo, handler);
	}
}
