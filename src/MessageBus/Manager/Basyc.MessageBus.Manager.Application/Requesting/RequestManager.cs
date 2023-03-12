using Basyc.Diagnostics.Shared.Durations;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics;
using Basyc.MessageBus.Manager.Application.ResultDiagnostics.Durations;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

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
		Requests = new ReadOnlyCollection<MessageContext>(requests);
	}

	//public Dictionary<MessageInfo, List<RequestContext>> Requests { get; } = new();
	private List<MessageContext> requests { get; } = new();
	public ReadOnlyCollection<MessageContext> Requests { get; }

	public MessageRequest StartRequest(RequestInput request)
	{
		var traceId = Interlocked.Increment(ref requestCounter).ToString().PadLeft(32, '0');
		var messageContext = Requests.FirstOrDefault(x => x.MessageInfo == request.MessageInfo);
		if (messageContext == default)
		{
			messageContext = new MessageContext(request.MessageInfo);
			requests.Add(messageContext);
		}

		var requestDiagnostics = requestDiagnosticsManager.CreateDiagnostics(traceId);
		requestDiagnostics.AddLog(requestManagerServiceIdentity, DateTimeOffset.UtcNow, LogLevel.Information, "Choosing requester", null);
		var requester = requesterSelector.PickRequester(request.MessageInfo);
		IDurationMapBuilder durationMapBuilder =
			new InMemoryDiagnosticsSourceDurationMapBuilder(requestManagerServiceIdentity, traceId, "root", inMemoryRequestDiagnosticsSource);
		var requestContext = new MessageRequest(request, DateTime.Now, traceId, durationMapBuilder, requestDiagnostics);
		messageContext.MessageRequests.Add(requestContext);
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
