using Basyc.Diagnostics.Shared.Durations;
using Basyc.MessageBus.Manager.Application.Initialization;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics.Durations;
using Microsoft.Extensions.Logging;

namespace Basyc.MessageBus.Manager.Application.Requesting;

public class RequestManager : IRequestManager
{
	private readonly InMemoryRequestDiagnosticsSource inMemoryRequestDiagnosticsSource;
	private readonly IRequestDiagnosticsManager requestDiagnosticsManager;
	private readonly IRequesterSelector requesterSelector;
	private readonly ServiceIdentity requestManagerServiceIdentity;
	private int requestCounter;

	public RequestManager(IRequesterSelector requesterSelector, IRequestDiagnosticsManager loggingManager,
		InMemoryRequestDiagnosticsSource inMemoryRequestDiagnosticsSource)
	{
		this.requesterSelector = requesterSelector;
		requestDiagnosticsManager = loggingManager;
		this.inMemoryRequestDiagnosticsSource = inMemoryRequestDiagnosticsSource;
		requestManagerServiceIdentity = ServiceIdentity.ApplicationWideIdentity;
	}

	public Dictionary<RequestInfo, List<RequestContext>> Results { get; } = new();

	public RequestContext StartRequest(Request request)
	{
		var traceId = Interlocked.Increment(ref requestCounter).ToString().PadLeft(32, '0');
		if (Results.TryGetValue(request.RequestInfo, out var requestContexts) is false)
		{
			requestContexts = new List<RequestContext>();
			Results.Add(request.RequestInfo, requestContexts);
		}

		var requestDiagnostics = requestDiagnosticsManager.CreateDiagnostics(traceId);
		requestDiagnostics.AddLog(requestManagerServiceIdentity, DateTimeOffset.UtcNow, LogLevel.Information, "Choosing requester", null);
		var requester = requesterSelector.PickRequester(request.RequestInfo);
		IDurationMapBuilder durationMapBuilder =
			new InMemoryDiagnosticsSourceDurationMapBuilder(requestManagerServiceIdentity, traceId, "root", inMemoryRequestDiagnosticsSource);
		var requestContext = new RequestContext(request, DateTime.Now, traceId, durationMapBuilder, requestDiagnostics);
		requestContexts.Add(requestContext);
		requestDiagnostics.AddLog(requestManagerServiceIdentity, DateTimeOffset.UtcNow, LogLevel.Information, "Giving request to requester", null);
		// var dummyStartSegment = durationMapBuilder.StartNewSegment("StartRequest1");
		// var startRequestActivity = DiagnosticHelper.Start("StartRequest2", dummyStartSegment.TraceId, dummyStartSegment.Id);
		requester.StartRequest(requestContext, new ResultLoggingContextLogger(requestManagerServiceIdentity, requestDiagnostics));
		// startRequestActivity.Stop();
		// dummyStartSegment.End();
		requestDiagnostics.AddLog(requestManagerServiceIdentity, DateTimeOffset.UtcNow, LogLevel.Information, "Requester finished", null);

		return requestContext;
	}
}
